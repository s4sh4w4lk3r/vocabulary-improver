using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using NgrokApi;
using Serilog;
using Telegram.Bot;
using ViApi.Services.Repository;
using ViApi.Services.Telegram;
using ViApi.Services.Telegram.UpdateHandlers;
using ViApi.Types.Configuration;

namespace ViApi.Services;

public static class DependencyRegistrationExtensions
{
    public static async Task RegisterDependencies(this WebApplicationBuilder builder, string[] args)
    {
        builder.RegisterSerilog();
        //Должен быть только один аргумент командной строки в виде путя до конфига.
        string configPath = args.FirstOrDefault()!.Throw("Не распознан путь до конфига.").IfNull(s => s);
        builder.Configuration.AddJsonFile(configPath);
        builder.Services.AddControllers().AddNewtonsoftJson();

        builder.RegisterDatabases();
        await builder.RegisterTelegramBot();
    }

    private static void RegisterDatabases(this WebApplicationBuilder builder)
    {
        var dbConf = builder.Configuration.GetRequiredSection("DbConfiguration").Get<DbConfiguration>()!;
        IMongoDatabase mongoDb = new MongoClient(dbConf.MongoDbConnString).GetDatabase(dbConf.MongoDbName);
        builder.Services.AddSingleton(mongoDb);
        builder.Services.AddDbContext<MySqlDbContext>(options => options.UseMySql(dbConf.MySqlConnString, ServerVersion.AutoDetect(dbConf.MySqlConnString)));
        builder.Services.AddTransient<IRepository, MySqlMongoRepository>();
    }
    private static async Task RegisterTelegramBot(this WebApplicationBuilder builder)
    {
        var tgConf = await SetWebhookUrl(builder);
        builder.Services.Configure<BotConfiguration>(builder.Configuration.GetRequiredSection("BotConfiguration"));

        builder.Services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient) => new TelegramBotClient(tgConf.BotToken, httpClient));

        builder.Services.AddScoped<UpdateHandler>();
        builder.Services.AddHostedService<ConfigureWebhook>();
    }
    private static async Task<BotConfiguration> SetWebhookUrl(this WebApplicationBuilder builder)
    {
        var ngrokConf = builder.Configuration.GetRequiredSection("NgrokConfiguration").Get<NgrokConfiguration>()!;
        var tgConf = builder.Configuration.GetRequiredSection("BotConfiguration").Get<BotConfiguration>()!;
        string url;

        if ((ngrokConf.IsRequired is false) && (string.IsNullOrWhiteSpace(tgConf.WebhookUrl) is false))
        {
            url = tgConf.WebhookUrl;
        }
        else if ((ngrokConf.IsRequired is true) && (string.IsNullOrWhiteSpace(ngrokConf.Token) is false))
        {
            url = await GetNgrokUrl(builder);
        }
        else throw new InvalidOperationException("Не получен Url для вебхука.");


        builder.Configuration.GetRequiredSection("BotConfiguration").GetRequiredSection("WebhookUrl").Value = url;
        tgConf = builder.Configuration.GetRequiredSection("BotConfiguration").Get<BotConfiguration>()!;
        return tgConf;
    }
    private static async Task<string> GetNgrokUrl(this WebApplicationBuilder builder, CancellationToken cancellationToken = default)
    {
        var ngrokConf = builder.Configuration.GetRequiredSection("NgrokConfiguration").Get<NgrokConfiguration>()!;

        ngrokConf.Token.ThrowIfNull().IfNullOrWhiteSpace(s => s);
        var ngrok = new Ngrok(ngrokConf.Token);

        var tunnel = await ngrok.Tunnels.List().FirstOrDefaultAsync(cancellationToken);
        string url = tunnel?.PublicUrl!;
        url.Throw(_ => new InvalidOperationException("Ngrok URL не получен от API.")).IfNullOrWhiteSpace(s => s);
        return url;
    }
    private static void RegisterSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
            .WriteTo.Console().CreateLogger();

        builder.Host.UseSerilog(Log.Logger);
    }
}
