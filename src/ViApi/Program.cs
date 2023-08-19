using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using NgrokApi;
using Telegram.Bot;
using Throw;
using ViApi.Database;
using ViApi.Extensions;
using ViApi.Validation;

namespace ViApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            string viApiConfigPath = @"C:\Users\sanchous\Desktop\ViApiConfig.json";
            builder.Configuration.AddJsonFile(viApiConfigPath);


            string mySqlConnString = builder.Configuration.GetRequiredSection("MySql").Value!;
            string mongoDbConnString = builder.Configuration.GetRequiredSection("MongoDb").Value!;
            string dbNameMongo = builder.Configuration.GetRequiredSection("dbNameMongo").Value!;
            string telegramBotToken = builder.Configuration.GetRequiredSection("BotToken").Value!;
            string ngrokToken = builder.Configuration.GetRequiredSection("ngrokToken").Value!;

            bool stringsOk = ViValidation.IsNotEmptyStrings(mySqlConnString, mongoDbConnString, dbNameMongo, telegramBotToken, ngrokToken);
            stringsOk.Throw(_ => new ArgumentException("Некоторые строковые значения из конфига не были распознаны."));


            IMongoDatabase mongoDb = new MongoClient(mongoDbConnString).GetDatabase(dbNameMongo);
            ITelegramBotClient telegramBotClient = new TelegramBotClient(telegramBotToken);
            Ngrok ngrok = new Ngrok(ngrokToken);

            builder.Services.AddSingleton<IMongoDatabase>(mongoDb);
            builder.Services.AddSingleton<ITelegramBotClient>(telegramBotClient);
            builder.Services.AddSingleton<Ngrok>(ngrok);
            builder.Services.AddDbContext<MySqlDbContext>(options => options.UseMySql(mySqlConnString, ServerVersion.AutoDetect(mySqlConnString)));

            var app = builder.Build();


            bool ok = await app.Services.EnsureServicesOkAsync(app.Logger);
            if (ok is false)
            {
                return;
            }

            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }

}