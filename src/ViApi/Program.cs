using ViApi.Extensions;

namespace ViApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.RegisterDependencies(args);

            var app = builder.Build();

            bool ok = await app.Services.EnsureServicesOkAsync(app.Logger);
            if (ok is false)
            {
                return;
            }
#error написать класс который выбирает реализацию для IUrlGetter или найти чьи-то примеры. Также попробовать написать реализацию для неNgrok
            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}