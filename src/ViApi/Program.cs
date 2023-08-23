using MongoDB.Driver;
using ViApi.Extensions;
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

        var user = mysql.Users.FirstOrDefault();
        await mysql.InsertDictionaryAsync(user, "name");
    }
}
#warning добавить завтра везде токены отмены чтобы опреации не зависали.