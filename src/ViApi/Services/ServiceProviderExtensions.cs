using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Serilog;
using Telegram.Bot;
using ViApi.Services.MySql;

namespace ViApi.Services
{
    public static class ServiceProviderExtensions
    {
        public async static Task EnsureServicesOkAsync(this IServiceProvider serviceProvider, int intervalToCancel = 10)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(intervalToCancel));
            var logger = Log.Logger;

            var ensureTgTask = EnsureTelegramBotAsync(serviceProvider, cts.Token);
            var ensureMySqlTask = EnsureMySqlAsync(serviceProvider, cts.Token);
            var ensureMongoDbTask = EnsureMongoDbAsync(serviceProvider, cts.Token);

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
                await ensureMySqlTask;
                Log.Information("MySQL {OK}.", "OK");
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, string.Empty);
                excepList.Add(ex);
            }

            try
            {
                await ensureMongoDbTask;
                Log.Information("MongoDb {OK}.", "OK");
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, string.Empty);
                excepList.Add(ex);
            }

            if (excepList.Count > 0) { throw new AggregateException(excepList); }
        }


        private static async Task<bool> EnsureMySqlAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            bool mySqlOk = await Task.Run(() =>
            {
                using var scoped = serviceProvider.CreateScope();
                var mySql = scoped.ServiceProvider.GetRequiredService<MySqlDbContext>();
                return mySql.Database.CanConnect();
            });
            return mySqlOk;
        }
        private static async Task<bool> EnsureMongoDbAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            try
            {
                var mongoDb = serviceProvider.GetRequiredService<IMongoDatabase>();
                var dbNamesCoursor = await mongoDb.Client.ListDatabaseNamesAsync(cancellationToken);
                var dbNamesList = await dbNamesCoursor.ToListAsync(cancellationToken);

                return dbNamesList.Contains("vocabulary-improver");
            }
            catch (Exception)
            {

                throw new InvalidOperationException("Не получен ответ от MongoDb");
            }
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
