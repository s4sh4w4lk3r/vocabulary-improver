using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Serilog;
using Telegram.Bot.Types;
using ViApi.Services.MongoDb;
using ViApi.Services.MySql;
using ViApi.Types.Common.Users;
using ViApi.Types.Telegram;

namespace ViApi.Services.Telegram.UpdateHandlers;

public class UserHandlers
{
    private readonly Update _update;
    private readonly MySqlDbContext _mysql;
    private readonly IMongoDatabase _mongo;
    private readonly CancellationToken _cancellationToken;
    public UserHandlers(Update update, MySqlDbContext mysql, IMongoDatabase mongo, CancellationToken cancellationToken = default)
    {
        _update = update;
        _mysql = mysql;
        _mongo = mongo;
        _cancellationToken = cancellationToken;
    }
    public async Task<TelegramSession> GetSessionAsync()
    {
        long chatId64 = _update.GetId();

        var session = await TryGetSessionFromMongoAsync(chatId64);
        if (session is not null) 
        {
            Log.Debug("Получена сессия из MongoDb {session}.", session);
            return session; 
        }
        Log.Debug("Сессия из MongoDb не получена. Попытка найти пользователя в MySQL.");

        var user = await TryGetUserFromSqlAsync(chatId64);
        if (user is not null)
        {
            session = new TelegramSession(user);
            await _mongo.InsertOrUpdateUserSessionAsync(session, _cancellationToken);
            Log.Debug("Пользователь найден в MySql и была создана сессия в MongoDb {session}.", session);
            return session;
        }

        return await RegisterUserAsync(chatId64, _update.GetFirstname());
    }

    private async Task<TelegramSession?> TryGetSessionFromMongoAsync(long chatId64)
    {
        return await _mongo.GetUserSessionAsync(chatId64, _cancellationToken);
    }
    private async Task<TelegramUser?> TryGetUserFromSqlAsync(long chatId64)
    {
        return await _mysql.Set<TelegramUser>().FirstOrDefaultAsync(u => u.TelegramId == chatId64);
    }
    private async Task<TelegramSession> RegisterUserAsync(long chatId64, string firstname)
    {
        Log.Debug("Пользователь не был найден, регистрируем в бд.");
        var user = new TelegramUser(Guid.NewGuid(), firstname, chatId64);
        var insertToMySqlTask = _mysql.InsertUserAsync(user);
        var session = new TelegramSession(user);
        if (await insertToMySqlTask is true)
        {
            await _mongo.InsertOrUpdateUserSessionAsync(session, _cancellationToken);
            Log.Debug("Пользователь был зареган {session}.", session);
            return session;
        }
        else
        {
            throw new InvalidOperationException("Не получилось зарегать польльзователя в MySql.");
        }
    }
}