using MongoDB.Bson;
using MongoDB.Driver;
using ViApi.Types.Common.Users;
using ViApi.Types.Telegram;

namespace ViApi.Services.MongoDb;

public static class MongoQueriesExtensions
{
    #region Публичные методы
    public static async Task<UserSession?> GetUserSessionAsync(this IMongoDatabase db, UserBase user)
    {
        var userGuid = user.Guid;
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = db.GetCollection<UserSession>("usersessions");

        var userSession = await collection.Find(filter).FirstOrDefaultAsync();
        return userSession;
    }

    public static async Task InsertOrUpdateUserSessionAsync(this IMongoDatabase db, UserSession userSession)
    {
        var userGuid = userSession.UserGuid;
        var filter = new BsonDocument { { "UserGuid", new BsonBinaryData(userGuid, GuidRepresentation.Standard) } };
        var collection = db.GetCollection<UserSession>("usersessions");

        await collection.ReplaceOneAsync(filter, userSession, new ReplaceOptions { IsUpsert = true });
    }
    #endregion

    #region Приватные методы

    #endregion
}