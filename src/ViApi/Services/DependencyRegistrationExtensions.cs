using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Telegram.Bot;
using ViApi.Services.GetUrlService;
using ViApi.Services.MySql;

namespace ViApi.Services;

public static class DependencyRegistrationExtensions
{
    public static void RegisterDependencies(this WebApplicationBuilder builder, string[] args)
    {
        //Должен быть только один аргумент командной строки в виде путя до конфига.
        string configPath = args.FirstOrDefault()!.Throw("Не распознан путь до конфига.").IfNull(s => s);
        builder.Configuration.AddJsonFile(configPath);

        builder.RegisterMySql();
        builder.RegisterMongoDb();
        builder.RegisterTelegramBot();
        builder.RegisterUrl();
        builder.Services.AddControllers();
    }
    private static void RegisterMongoDb(this WebApplicationBuilder builder)
    {
        string mongoDbConnString = builder.Configuration.GetRequiredSection("MongoDb").Value!;
        string dbNameMongo = builder.Configuration.GetRequiredSection("dbNameMongo").Value!;

        mongoDbConnString.Throw("mongoDbConnString не получен.").IfNullOrWhiteSpace(s => s);
        dbNameMongo.Throw("DbNameMongo не получен.").IfNullOrWhiteSpace(s => s);

        IMongoDatabase mongoDb = new MongoClient(mongoDbConnString).GetDatabase(dbNameMongo);

        builder.Services.AddSingleton(mongoDb);
    }
    private static void RegisterMySql(this WebApplicationBuilder builder)
    {
        string mySqlConnString = builder.Configuration.GetRequiredSection("MySql").Value!;

        mySqlConnString.Throw("MySqlConnString не получен.").IfNullOrWhiteSpace(s => s);

        builder.Services.AddDbContext<MySqlDbContext>(options => options.UseMySql(mySqlConnString, ServerVersion.AutoDetect(mySqlConnString)));
    }
    private static void RegisterTelegramBot(this WebApplicationBuilder builder)
    {
        string telegramBotToken = builder.Configuration.GetRequiredSection("BotToken").Value!;

        telegramBotToken.Throw("TelegramBotToken не получен.").IfNullOrWhiteSpace(s => s);

        ITelegramBotClient telegramBotClient = new TelegramBotClient(telegramBotToken);
        builder.Services.AddSingleton(telegramBotClient);
    }
    private static void RegisterUrlGetterNgrok(this WebApplicationBuilder builder)
    {
        string ngrokToken = builder.Configuration.GetRequiredSection("ngrokToken").Value!;

        ngrokToken.Throw("NgrokApiToken не получен.").IfNullOrWhiteSpace(s => s);

        IUrlGetter ngrokService = new UrlGetterFromNgrok(ngrokToken);
        builder.Services.AddSingleton(ngrokService);
    }
    private static void RegisterUrlGetterFromConfig(this WebApplicationBuilder builder)
    {
        string url = builder.Configuration.GetRequiredSection("PublicURL").Value!;
        url.Throw("PublicURL не получен из конфига.").IfNullOrWhiteSpace(s => s);
        IUrlGetter getter = new UrlGetterFromConfig(url);
        builder.Services.AddSingleton(getter);
    }
    private static void RegisterUrl(this WebApplicationBuilder builder)
    {
        if (builder.Environment.IsDevelopment())
        {
            builder.RegisterUrlGetterNgrok();
        }
        if (builder.Environment.IsProduction())
        {
            builder.RegisterUrlGetterFromConfig();
        }
    }
}
