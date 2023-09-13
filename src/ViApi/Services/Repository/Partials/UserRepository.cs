using FluentValidation;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using System.Net.Mail;
using ViApi.Types.Common.Users;
using ViApi.Types.Telegram;
using ViApi.Validation.Fluent;
using ViApi.Validation.Fluent.UserValidation;

namespace ViApi.Services.Repository;

public partial class RepositoryClass
{
    public async Task<TelegramSession> GetOrRegisterSessionAsync(long chatId64, string firstname, CancellationToken cancellationToken = default)
    {
        chatId64.Throw().IfDefault();
        firstname.Throw().IfNullOrWhiteSpace(_=>_);

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
    public async Task<ApiUser?> GetValidUserAsync(string username, CancellationToken cancellationToken = default)
    {
        username.Throw().IfNullOrWhiteSpace(_ => _);

        var validUser = await _mysql.Set<ApiUser>().FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        return validUser;
    }
    public async Task<ApiUser?> GetValidUserAsync(MailAddress email, CancellationToken cancellationToken = default)
    {
        email.ThrowIfNull();

        var validUser = await _mysql.Set<ApiUser>().FirstOrDefaultAsync(u => u.Email == email.Address, cancellationToken);
        return validUser;
    }
    public async Task<ApiUser?> GetValidUserAsync(Guid userGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw().IfDefault();

        var validUser = await _mysql.Set<ApiUser>().FirstOrDefaultAsync(u => u.Guid == userGuid, cancellationToken);
        return validUser;
    }
    public async Task<bool> InsertUserAsync(UserBase user, CancellationToken cancellationToken = default)
    {
        if (user is TelegramUser telegramUser)
        {
            await new TelegramUserValidator().ValidateAndThrowAsync(telegramUser, cancellationToken);
            bool exists = await _mysql.Set<TelegramUser>().AnyAsync(u => u.TelegramId == telegramUser.TelegramId, cancellationToken);
            if (exists) return false;
        }
        if (user is ApiUser apiUser)
        {
            await new ApiUserValidator().ValidateAndThrowAsync(apiUser, cancellationToken);
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
        await new TelegramSessionValidator().ValidateAndThrowAsync(userSession, cancellationToken);
        var userGuid = userSession.UserGuid;
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = _mongo.GetCollection<TelegramSession>("usersessions");

        await collection.ReplaceOneAsync(filter, userSession, new ReplaceOptions { IsUpsert = true }, cancellationToken);
    }
    public async Task<bool> DeleteUserAndSessionAsync(TelegramSession session, CancellationToken cancellationToken = default)
    {
        await new TelegramSessionValidator().ValidateAndThrowAsync(session, cancellationToken);
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
    public async Task<bool> IsUserExists(Guid userGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw().IfDefault();
        return await _mysql.Users.AnyAsync(u => u.Guid == userGuid, cancellationToken);
    }
    public async Task UpdateApiUser(ApiUser user, CancellationToken token = default)
    {
        user.Guid.Throw().IfDefault();
        var trackedUser = await _mysql.Set<ApiUser>().FirstOrDefaultAsync(u => u.Guid == user.Guid, cancellationToken: token);
        if (trackedUser is not null)
        {
            trackedUser.Email = user.Email;
            trackedUser.Firstname = user.Firstname;
            trackedUser.Password = user.Password;
            trackedUser.Username = user.Username;
            await _mysql.SaveChangesAsync(token);
        }
    }

    public async Task<bool> DeleteApiUserAsync(Guid userGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw().IfDefault();
        var userToDel = await _mysql.Set<ApiUser>().Where(u => u.Guid == userGuid).FirstOrDefaultAsync(cancellationToken);
        if (userToDel is not null)
        {
            _mysql.Set<ApiUser>().Remove(userToDel);
            await _mysql.SaveChangesAsync(cancellationToken);
            return true;
        }
        else return false;
    }

    private async Task DeleteUserSessionAsync(TelegramSession userSession, CancellationToken cancellationToken = default)
    {
        await new TelegramSessionValidator().ValidateAndThrowAsync(userSession, cancellationToken);
        var userGuid = userSession.UserGuid;
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = _mongo.GetCollection<TelegramSession>("usersessions");

        await collection.DeleteOneAsync(filter, cancellationToken);
    }
    private async Task<TelegramSession> RegisterUserAsync(long chatId64, string firstname, CancellationToken cancellationToken = default)
    {
        chatId64.Throw().IfDefault();
        firstname.Throw().IfNullOrWhiteSpace(_ => _);

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
    private async Task<TelegramSession?> GetUserSessionAsync(long chatId64, CancellationToken cancellationToken = default)
    {
        chatId64.Throw().IfDefault();
        var filter = new BsonDocument { { "TelegramId", chatId64 } };
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
