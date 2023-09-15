using Serilog;
using Telegram.Bot;
using ViApi.Services.Repository;

namespace ViApi.Services
{
    public static class ServiceProviderExtensions
    {
        public async static Task EnsureServicesOkAsync(this IServiceProvider serviceProvider, int intervalToCancel = 5)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(intervalToCancel));
            var logger = Log.Logger;

            var ensureTgTask = EnsureTelegramBotAsync(serviceProvider, cts.Token);
            var ensureDb = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IRepository>().EnsureDatabasesAsync(cts.Token);

            var excepList = new List<Exception>();

            try
            {
                await ensureTgTask;
                Log.Information("TelegramBot {OK}.", "OK");
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, string.Empty);
                excepList.Add(ex);
            }

            try
            {
                await ensureDb;
                Log.Information("Database {OK}.", "OK");
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, string.Empty);
                excepList.Add(ex);
            }

            if (excepList.Count > 0) { throw new AggregateException(excepList); }
        }
        private static async Task<bool> EnsureTelegramBotAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            using var scope = serviceProvider.CreateScope();
            var bot = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            var name = (await bot.GetMyNameAsync(cancellationToken: cancellationToken)).Name;
            return string.IsNullOrWhiteSpace(name) is false;
        }
    }
}
