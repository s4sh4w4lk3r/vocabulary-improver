using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Telegram.Bot;
using Throw;
using ViApi.Database;
using ViApi.Services.GetUrlService;

namespace ViApi.Extensions;

public static class DependencyRegistration
{
    public static void RegisterDependencies(this WebApplicationBuilder builder, string[] args)
    {
        //Должен быть только один аргумент командной строки в виде путя до конфига.
        string configPath = args.FirstOrDefault()!.Throw("Не распознан путь до конфига.").IfNull(s => s);
        builder.Configuration.AddJsonFile(configPath);

        builder.RegisterMySql();
        builder.RegisterMongoDb();
        builder.RegisterTelegramBot();
        var urlSerivce = builder.RegisterNgrok();
        string url = urlSerivce.GetUrl();
        Console.WriteLine($"Публичный URL: {url}");
    }
    private static IMongoDatabase RegisterMongoDb(this WebApplicationBuilder builder)
    {
        string mongoDbConnString = builder.Configuration.GetRequiredSection("MongoDb").Value!;
        string dbNameMongo = builder.Configuration.GetRequiredSection("dbNameMongo").Value!;

        mongoDbConnString.Throw("MySqlConnString не получен.").IfWhiteSpace();
        dbNameMongo.Throw("DbNameMongo не получен.").IfWhiteSpace();

        IMongoDatabase mongoDb = new MongoClient(mongoDbConnString).GetDatabase(dbNameMongo);
        builder.Services.AddSingleton(mongoDb);

        return mongoDb;
    }
    private static void RegisterMySql(this WebApplicationBuilder builder)
    {
        string mySqlConnString = builder.Configuration.GetRequiredSection("MySql").Value!;

        mySqlConnString.Throw("MySqlConnString не получен.").IfWhiteSpace();

        builder.Services.AddDbContext<MySqlDbContext>(options => options.UseMySql(mySqlConnString, ServerVersion.AutoDetect(mySqlConnString)));
    }
    private static ITelegramBotClient RegisterTelegramBot(this WebApplicationBuilder builder)
    {
        string telegramBotToken = builder.Configuration.GetRequiredSection("BotToken").Value!;

        telegramBotToken.Throw("TelegramBotToken не получен.").IfWhiteSpace();

        ITelegramBotClient telegramBotClient = new TelegramBotClient(telegramBotToken);
        builder.Services.AddSingleton(telegramBotClient);

        return telegramBotClient;
    }
    private static IUrlGetter RegisterNgrok(this WebApplicationBuilder builder)
    {
        string ngrokToken = builder.Configuration.GetRequiredSection("ngrokToken").Value!;

        ngrokToken.Throw("NgrokApiToken не получен.").IfWhiteSpace();

        IUrlGetter ngrokService = new NgrokUrlGetter(ngrokToken);
        builder.Services.AddSingleton(ngrokService);

        return ngrokService;
    }
}
