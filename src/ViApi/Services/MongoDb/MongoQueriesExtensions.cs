using MongoDB.Bson;
using MongoDB.Driver;
using ViApi.Types.Telegram;

namespace ViApi.Services.MongoDb;

public static class MongoQueriesExtensions
{
    public static async Task<TelegramSession?> GetUserSessionAsync(this IMongoDatabase db, Guid userGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw().IfDefault();
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = db.GetCollection<TelegramSession>("usersessions");

        var userSession = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return userSession;
    }

    public static async Task InsertOrUpdateUserSessionAsync(this IMongoDatabase db, TelegramSession userSession, CancellationToken cancellationToken = default)
    {
        var userGuid = userSession.UserGuid;
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = db.GetCollection<TelegramSession>("usersessions");

        await collection.ReplaceOneAsync(filter, userSession, new ReplaceOptions { IsUpsert = true }, cancellationToken);
    }

    public static async Task<TelegramSession?> GetUserSessionAsync(this IMongoDatabase db, long telegramId, CancellationToken cancellationToken = default)
    {
        telegramId.Throw().IfDefault();
        var filter = new BsonDocument { { "TelegramId", telegramId } };
        var collection = db.GetCollection<TelegramSession>("usersessions");

        var userSession = await collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        return userSession;
    }
    public static async Task DeleteUserSessionAsync(this IMongoDatabase db, TelegramSession userSession, CancellationToken cancellationToken = default)
    {
        var userGuid = userSession.UserGuid;
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = db.GetCollection<TelegramSession>("usersessions");

        await collection.DeleteOneAsync(filter, cancellationToken);
    }
}