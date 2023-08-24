using MongoDB.Driver;
using ViApi.Services;
using ViApi.Services.MySql;

namespace ViApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.RegisterDependencies(args);

        var app = builder.Build();

        await app.Services.EnsureServicesOkAsync(app.Logger);


        using IServiceScope scope = app.Services.CreateScope();
        var mongodb = app.Services.GetMongoDb();
        using var mysql = scope.ServiceProvider.GetRequiredService<MySqlDbContext>();

        var dictGuid = Guid.Parse("ee4a74d9-d859-49e0-9063-de9672433ff4");
        var userGuid = Guid.Parse("f7f22571-0f56-4b1e-8f19-1e0eb0754ff2");

        var list = await mysql.GetWordsAsync(userGuid, dictGuid);
    }
}