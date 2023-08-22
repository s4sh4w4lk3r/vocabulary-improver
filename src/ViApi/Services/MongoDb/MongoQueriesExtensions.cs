using MongoDB.Bson;
using MongoDB.Driver;
using ViApi.Types.Common.Users;
using ViApi.Types.Telegram;

namespace ViApi.Services.MongoDb;

public static class MongoQueriesExtensions
{
    public static async Task<UserSession?> GetUserSessionAsync(this IMongoDatabase db, UserBase user)
    {
        var userGuid = user.Guid;
        var filter = new BsonDocument { { "UserGuid", userGuid.ToBson() } };
        var collection = db.GetCollection<UserSession>("usersessions");

        var userSession = await collection.Find(filter).FirstOrDefaultAsync();
        return userSession;
    }
    public static async Task<bool> InsertOrUpdateUserSessionAsync(this IMongoDatabase db, UserSession userSession)
    {
        throw new NotImplementedException();
    }
}