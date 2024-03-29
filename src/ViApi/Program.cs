﻿using ViApi.Services;

namespace ViApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        await builder.RegisterServices();
        var app = builder.Build();
        await app.Services.EnsureServicesOkAsync();

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}