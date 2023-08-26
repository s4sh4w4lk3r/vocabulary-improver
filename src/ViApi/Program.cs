using ViApi.Services;
using ViApi.Services.MongoDb;
using ViApi.Types.Telegram;

namespace ViApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        await builder.RegisterDependencies(args);
        var app = builder.Build();
        await app.Services.EnsureServicesOkAsync();
        app.MapControllers();
        app.Run();
    }
}