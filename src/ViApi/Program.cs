using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using ViApi.Extensions;
using ViApi.Services.MySql;
using ViApi.Types.Common.Users;
using ViApi.Types.Telegram;

namespace ViApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.RegisterDependencies(args);

            var app = builder.Build();

            bool ok = await app.Services.EnsureServicesOkAsync(app.Logger);
            if (ok is false)
            {
                return;
            }

            var mongodb = app.Services.GetMongoDb();
            using var mysql = new MySqlDbContext(new DbContextOptionsBuilder<MySqlDbContext>().UseMySql("",
                ServerVersion.AutoDetect("")).Options);


            var user = mysql.Users.First();
            var dict = mysql.Dictionaries.First();
            var words = await mysql.Words.ToListAsync();
            var session = new UserSession((user as TelegramUser).Guid, dict.Guid, new Stack<Types.Common.Word>(words));



            var usersessions = mongodb.GetCollection<UserSession>("usersessions");
            usersessions.InsertOne(session);

            using var cursor = await usersessions.FindAsync<UserSession>(new BsonDocument());

            var list = await cursor.ToListAsync();

#error попоробовать задать scoped контекст и уже пилить методы для бд

            /*            await usersessions.InsertOneAsync(user);*/
            /*
                        app.MapGet("/", () => "Hello World!");

                        app.Run()*/
            ;

        }
    }
}