using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using ViApi.Types.Common;
using ViApi.Types.Common.Users;
using ViApi.Types.Telegram;
using ViApi.Types.Users;

namespace ViApi.Services.Repository;

public class MySqlMongoRepository : IRepository
{
    private readonly MySqlDbContext _mysql;
    private readonly IMongoDatabase _mongo;

    public MySqlMongoRepository(MySqlDbContext mysql, IMongoDatabase mongo)
    {
        _mysql = mysql;
        _mongo = mongo;
    }

    public async Task<bool> CheckDictionaryIsExistAsync(Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default)
    {
        return (await _mysql.Dictionaries.Where(d => d.Guid == dictGuid && d.UserGuid == userGuid).CountAsync(cancellationToken: cancellationToken)) == 1;
    }
    public async Task<bool> DeleteDictionaryAsync(Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод DeleteDictionaryAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод DeleteDictionaryAsync пришел пустой dictGuid").IfDefault();

        var dictToDel = await _mysql.Dictionaries.Where(d => d.Guid == dictGuid && d.UserGuid == userGuid).FirstOrDefaultAsync(cancellationToken);
        if (dictToDel is null) { return false; }

        _mysql.Dictionaries.Remove(dictToDel);
        await _mysql.SaveChangesAsync(cancellationToken);
        return true;
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
    public async Task DeleteUserSessionAsync(TelegramSession userSession, CancellationToken cancellationToken = default)
    {
        var userGuid = userSession.UserGuid;
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = _mongo.GetCollection<TelegramSession>("usersessions");

        await collection.DeleteOneAsync(filter, cancellationToken);
    }
    public async Task<bool> DeleteWordAsync(Guid userGuid, Guid dictGuid, Guid wordGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод DeleteWordAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод DeleteWordAsync пришел пустой dictGuid").IfDefault();
        wordGuid.Throw("В метод DeleteWordAsync пришел пустой wordGuid").IfDefault();

        var wordToDel = await _mysql.Words.Include(w => w.Dictionary).FirstOrDefaultAsync(w => w.DictionaryGuid == dictGuid && w.Guid == wordGuid && w.Dictionary!.UserGuid == userGuid, cancellationToken);
        if (wordToDel is null) { return false; };

        _mysql.Words.Remove(wordToDel);
        await _mysql.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<List<Dictionary>> GetDicionariesList(Guid userGuid, CancellationToken cancellationToken = default)
    {
        return await _mysql.Dictionaries.Where(d => d.UserGuid == userGuid).ToListAsync(cancellationToken: cancellationToken);
    }
    public async Task<TelegramSession> GetOrRegisterSessionAsync(long chatId64, string firstname, CancellationToken cancellationToken = default)
    {
        var session = await GetUserSessionAsync(chatId64, cancellationToken);
        if (session is not null)
        {
            Log.Debug("Получена сессия из MongoDb {session}.", session);
            return session;
        }
        Log.Debug("Сессия из MongoDb не получена. Попытка найти пользователя в MySQL.");

        var user = await _mysql.Set<TelegramUser>().FirstOrDefaultAsync(u => u.TelegramId == chatId64, cancellationToken);
        if (user is not null)
        {
            session = new TelegramSession(user);
            await InsertOrUpdateUserSessionAsync(session, cancellationToken);
            Log.Debug("Пользователь найден в MySql и была создана сессия в MongoDb {session}.", session);
            return session;
        }

        return await RegisterUserAsync(chatId64, firstname);
    }
    public async Task<TelegramSession?> GetUserSessionAsync(Guid userGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw().IfDefault();
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = _mongo.GetCollection<TelegramSession>("usersessions");

        var userSession = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return userSession;
    }
    public async Task<TelegramSession?> GetUserSessionAsync(long telegramId, CancellationToken cancellationToken = default)
    {
        telegramId.Throw().IfDefault();
        var filter = new BsonDocument { { "TelegramId", telegramId } };
        var collection = _mongo.GetCollection<TelegramSession>("usersessions");

        var userSession = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return userSession;
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
    public async Task<List<Word>?> GetWordsAsync(Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод GetWordsAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод GetWordsAsync пришел пустой dictGuid").IfDefault();

        var dict = await _mysql.Dictionaries.Include(d => d.Words).FirstOrDefaultAsync(d => d.Guid == dictGuid && d.UserGuid == userGuid, cancellationToken);

        var words = dict?.Words;

        return words;
    }
    public async Task<bool> InsertDictionaryAsync(Dictionary dictionary, CancellationToken cancellationToken = default)
    {
        dictionary.UserGuid.Throw("В метод InsertDictionaryAsync пришел пустой Guid").IfDefault();
        dictionary.Name.Throw("В метод InsertDictionaryAsync пришло пустое имя").IfNullOrWhiteSpace(n => n);
        bool userExists = await _mysql.Users.AnyAsync(u => u.Guid == dictionary.UserGuid, cancellationToken);
        if (userExists is false) { return false; }

        await _mysql.Dictionaries.AddAsync(dictionary, cancellationToken);
        int stringsChanged = await _mysql.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task InsertOrUpdateUserSessionAsync(TelegramSession userSession, CancellationToken cancellationToken = default)
    {
        var userGuid = userSession.UserGuid;
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = _mongo.GetCollection<TelegramSession>("usersessions");

        await collection.ReplaceOneAsync(filter, userSession, new ReplaceOptions { IsUpsert = true }, cancellationToken);
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
    public async Task<bool> InsertWordAsync(Guid userGuid, Guid dictGuid, string sourceWord, string targetWord, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод InsertWordAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод InsertWordAsync пришел пустой dictGuid").IfDefault();
        sourceWord.Throw("В метод InsertDictionaryAsync пришло пустое sourceWord").IfNullOrWhiteSpace(n => n);
        targetWord.Throw("В метод InsertDictionaryAsync пришло пустое targetWord").IfNullOrWhiteSpace(n => n);

        var dictExist = await _mysql.Dictionaries.AnyAsync(d => d.Guid == dictGuid && d.UserGuid == userGuid, cancellationToken);
        if (dictExist is false) { return false; }

        await _mysql.Words.AddAsync(new Word(Guid.NewGuid(), sourceWord, targetWord, dictGuid), cancellationToken);
        await _mysql.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<bool> RenameDictionaryAsync(Guid userGuid, Guid dictGuid, string newName, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод RenameDictionaryAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод RenameDictionaryAsync пришел пустой dictGuid").IfDefault();
        newName.Throw("В метод RenameDictionaryAsync пришло пустое имя").IfNullOrWhiteSpace(n => n);
        var dictToRemove = await _mysql.Dictionaries.Where(d => d.Guid == dictGuid && d.UserGuid == userGuid).FirstOrDefaultAsync(cancellationToken);
        if (dictToRemove is null) { return false; }

        dictToRemove.Name = newName;
        await _mysql.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task UpdateWordRating(Guid userGuid, Guid dictGuid, Guid wordGuid, RatingAction action, CancellationToken cancellationToken = default)
    {
        var word = await _mysql.Words.FirstOrDefaultAsync(w => w.Guid == wordGuid && w.Dictionary!.Guid == dictGuid && w.Dictionary.UserGuid == userGuid, cancellationToken);

        if (word is null) { return; }

        if (action is RatingAction.Increase)
        {
            word.IncreaseRating();
        }
        if (action is RatingAction.Increase)
        {
            word.DecreaseRating();
        }

        await _mysql.SaveChangesAsync();
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
}