using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using ViApi.Types.Common.Users;
using ViApi.Types.Telegram;
using ViApi.Types.Users;

namespace ViApi.Services.Repository;

public partial class TgRepository
{
    public async Task<TelegramSession> GetOrRegisterSessionAsync(long chatId64, string firstname, CancellationToken cancellationToken = default)
    {
        var session = await GetUserSessionAsync(chatId64, cancellationToken);
        if (session is not null)
        {
            Log.Information("Сессия получена из MongoDb: {session}", session);
            return session;
        }

        var user = await _mysql.Set<TelegramUser>().FirstOrDefaultAsync(u => u.TelegramId == chatId64, cancellationToken);
        if (user is not null)
        {
            session = new TelegramSession(user);
            await InsertOrUpdateUserSessionAsync(session, cancellationToken);
            Log.Information("Юзер получен из MySql, сессия создана: {session}", session);
            return session;
        }

        session = await RegisterUserAsync(chatId64, firstname, cancellationToken);
        Log.Information("Юзер зарегался и получена сессия: {session}", session);
        return session;
    }
    public async Task<ApiUser?> GetValidUserAsync(string username, string email, CancellationToken cancellationToken = default)
    {
        const string INVALID_TEMP_PASSWORD = "Pa55sword!";
        const string INVALID_TEMP_FIRSTNAME = "Billy";

        //Создается временный юзер, чтобы введенные в параметр данные прошли проверку.
        var tempUser = new ApiUser(Guid.NewGuid(), INVALID_TEMP_FIRSTNAME, username, email, INVALID_TEMP_PASSWORD);
        var validUser = await _mysql.Set<ApiUser>().FirstOrDefaultAsync(u => u.Email == tempUser.Email && u.Username == tempUser.Username, cancellationToken);
        return validUser;
    }
    public async Task<bool> InsertUserAsync(UserBase user, CancellationToken cancellationToken = default)
    {
        if (user is TelegramUser telegramUser)
        {
            bool exists = await _mysql.Set<TelegramUser>().AnyAsync(u => u.TelegramId == telegramUser.TelegramId, cancellationToken);
            if (exists) return false;
        }
        if (user is ApiUser apiUser)
        {
            bool exists = await _mysql.Set<ApiUser>().AnyAsync(u => u.Username == apiUser.Username || u.Email == apiUser.Email, cancellationToken);
            if (exists) return false;
        }
        await _mysql.Users.AddAsync(user, cancellationToken);
        int stringsChanged = await _mysql.SaveChangesAsync(cancellationToken);

        if (stringsChanged == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public async Task InsertOrUpdateUserSessionAsync(TelegramSession userSession, CancellationToken cancellationToken = default)
    {
        var userGuid = userSession.UserGuid;
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = _mongo.GetCollection<TelegramSession>("usersessions");

        await collection.ReplaceOneAsync(filter, userSession, new ReplaceOptions { IsUpsert = true }, cancellationToken);
    }
    public async Task<bool> DeleteUserAndSessionAsync(TelegramSession session, CancellationToken cancellationToken = default)
    {
        var user = await _mysql.Users.FirstOrDefaultAsync(u => u.Guid == session.UserGuid, cancellationToken);
        if (user is not null)
        {
            await DeleteUserSessionAsync(session, cancellationToken);
            _mysql.Users.Remove(user);
            await _mysql.SaveChangesAsync(cancellationToken);
            return true;
        }
        else return false;
    }
    
    private async Task DeleteUserSessionAsync(TelegramSession userSession, CancellationToken cancellationToken = default)
    {
        var userGuid = userSession.UserGuid;
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = _mongo.GetCollection<TelegramSession>("usersessions");

        await collection.DeleteOneAsync(filter, cancellationToken);
    }
    private async Task<TelegramSession> RegisterUserAsync(long chatId64, string firstname, CancellationToken cancellationToken = default)
    {
        Log.Debug("Пользователь не был найден, регистрируем в бд.");
        var user = new TelegramUser(Guid.NewGuid(), firstname, chatId64);
        var insertToMySqlTask = InsertUserAsync(user, cancellationToken);
        var session = new TelegramSession(user);
        if (await insertToMySqlTask is true)
        {
            await InsertOrUpdateUserSessionAsync(session, cancellationToken);
            Log.Debug("Пользователь был зареган {session}.", session);
            return session;
        }
        else
        {
            throw new InvalidOperationException("Не получилось зарегать польльзователя в MySql.");
        }
    }
    private async Task<TelegramSession?> GetUserSessionAsync(long telegramId, CancellationToken cancellationToken = default)
    {
        telegramId.Throw().IfDefault();
        var filter = new BsonDocument { { "TelegramId", telegramId } };
        var collection = _mongo.GetCollection<TelegramSession>("usersessions");

        var userSession = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return userSession;
    }
    private async Task<TelegramSession?> GetUserSessionAsync(Guid userGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw().IfDefault();
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = _mongo.GetCollection<TelegramSession>("usersessions");

        var userSession = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return userSession;
    }
}
