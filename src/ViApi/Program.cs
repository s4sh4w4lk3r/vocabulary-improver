using ViApi.Contollers;
using ViApi.Services;
using ViApi.Services.Telegram;

namespace ViApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        await builder.RegisterDependencies(args);
        var app = builder.Build();
        await app.Services.EnsureServicesOkAsync(app.Logger);

        app.MapBotWebhookRoute<BotController>(route: "/bot");
        app.MapControllers();
        app.Run(); ;
    }
}