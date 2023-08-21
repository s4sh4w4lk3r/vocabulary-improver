using ViApi.Types.Common;

namespace ViApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            new Word(Guid.NewGuid(), "f", "f", Guid.NewGuid(), -1);
            /*var builder = WebApplication.CreateBuilder(args);
            builder.RegisterDependencies(args);

            var app = builder.Build();

            bool ok = await app.Services.EnsureServicesOkAsync(app.Logger);
            if (ok is false)
            {
                return;
            }

            
            app.MapGet("/", () => "Hello World!");

            app.Run();*/

        }
    }
}