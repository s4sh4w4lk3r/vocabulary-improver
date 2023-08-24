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

    }
}