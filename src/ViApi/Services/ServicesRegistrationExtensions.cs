﻿using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using NgrokApi;
using Serilog;
using Telegram.Bot;
using ViApi.Services.EmailService;
using ViApi.Services.Repository;
using ViApi.Services.Telegram;
using ViApi.Services.Telegram.UpdateHandlers;
using ViApi.Types.Configuration;
using ViApi.Validation.Fluent;

namespace ViApi.Services;

public static class ServicesRegistrationExtensions
{
    public static async Task RegisterServices(this WebApplicationBuilder builder)
    {
        builder.RegisterSerilog();

        builder.Services.AddControllers().AddNewtonsoftJson();

        builder.RegisterAuth();
        builder.RegisterDatabases();
        await builder.RegisterTelegramBot();
    }

    private static void RegisterDatabases(this WebApplicationBuilder builder)
    {
        var dbConf = builder.Configuration.GetRequiredSection("DbConfiguration").Get<DbConfiguration>()!;
        new DbValidation().ValidateAndThrow(dbConf);

        IMongoDatabase mongoDb = new MongoClient(dbConf.MongoDbConnString).GetDatabase(dbConf.MongoDbName);
        builder.Services.AddSingleton(mongoDb);
        builder.Services.AddDbContext<MySqlDbContext>(options => options.UseMySql(dbConf.MySqlConnString, ServerVersion.AutoDetect(dbConf.MySqlConnString)));
        builder.Services.AddTransient<IRepository, RepositoryClass>();

        builder.Services.AddTransient<IEmailClient, EmailZaglushka>();
    }
    private static async Task RegisterTelegramBot(this WebApplicationBuilder builder)
    {
        var tgConf = await SetWebhookUrl(builder);

        builder.Services.Configure<BotConfiguration>(builder.Configuration.GetRequiredSection("BotConfiguration"));

        builder.Services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient) => new TelegramBotClient(tgConf.BotToken!, httpClient));

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
        new BotValidation().ValidateAndThrow(tgConf);
        return tgConf;
    }
    private static async Task<string> GetNgrokUrl(this WebApplicationBuilder builder, CancellationToken cancellationToken = default)
    {
        var ngrokConf = builder.Configuration.GetRequiredSection("NgrokConfiguration").Get<NgrokConfiguration>()!;

        ngrokConf.Token.ThrowIfNull().IfNullOrWhiteSpace(s => s);
        var ngrok = new Ngrok(ngrokConf.Token);

        var tunnel = await ngrok.Tunnels.List().FirstOrDefaultAsync(cancellationToken);
        string? url = tunnel?.PublicUrl.OriginalString;
        url.ThrowIfNull(_ => new InvalidOperationException("Ngrok URL не получен от API.")).IfNullOrWhiteSpace(s => s);
        return url;
    }
    private static void RegisterSerilog(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration().MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
            .MinimumLevel.Override("System.Net.Http.HttpClient", Serilog.Events.LogEventLevel.Warning)
            .WriteTo.Console().CreateLogger();

        builder.Host.UseSerilog(Log.Logger);
    }
    private static void RegisterAuth(this WebApplicationBuilder builder)
    {
        var jwtConf = builder.Configuration.GetRequiredSection("JwtConfiguration").Get<JwtConfiguration>()!;
        new JwtValidation().ValidateAndThrow(jwtConf);

        builder.Services.Configure<JwtConfiguration>(builder.Configuration.GetRequiredSection("JwtConfiguration"));

        builder.Services.AddAuthorization();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = "JWT_OR_COOKIE";
            options.DefaultChallengeScheme = "JWT_OR_COOKIE";
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidIssuer = jwtConf.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtConf.Audience,
                ValidateLifetime = true,
                IssuerSigningKey = jwtConf.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true
            };
        })
        .AddCookie("Cookies", options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
            options.Cookie.MaxAge = options.ExpireTimeSpan;
            options.SlidingExpiration = true;
        })
        .AddPolicyScheme("JWT_OR_COOKIE", "JWT_OR_COOKIE", options =>
        {
            options.ForwardDefaultSelector = context =>
            {
                if (context.Request.Cookies.ContainsKey(".AspNetCore.Cookies"))
                {
                    return CookieAuthenticationDefaults.AuthenticationScheme;
                }
                else
                {
                    return JwtBearerDefaults.AuthenticationScheme;
                }
            };
        });
    }
}