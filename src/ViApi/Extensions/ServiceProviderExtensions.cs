using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Telegram.Bot;
using ViApi.Services.GetUrlService;
using ViApi.Services.MySql;

namespace ViApi.Extensions
{
    public static class ServiceProviderExtensions
    {
        #region Публичные методы.
        public static IMongoDatabase GetMongoDb(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IMongoDatabase>();
        }

        public static ITelegramBotClient GetBotClient(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<ITelegramBotClient>();
        }

        public static IConfiguration GetConfiguration(this IServiceProvider serviceProvider)
        {
            return serviceProvider.GetRequiredService<IConfiguration>();
        }

        public async static Task<bool> EnsureServicesOkAsync(this IServiceProvider serviceProvider, ILogger logger)
        {
            bool mySqlOk = false;
            bool mongoDbOk = false;
            bool botOk = false;
            bool urlOk = false;

            try
            {

                botOk = await EnsureTelegramBotAsync(serviceProvider);
            }
            catch (Exception ex) { logger.LogCritical(ex, string.Empty); }


            try
            {
                mySqlOk = await EnsureMySqlAsync(serviceProvider);
            }
            catch (Exception ex) { logger.LogCritical(ex, string.Empty); }


            try
            {
                mongoDbOk = await EnsureMongoDbAsync(serviceProvider);
            }
            catch (Exception ex) { logger.LogCritical(ex, string.Empty); }

            try
            {
                urlOk = EnsureUrlGetted(serviceProvider, out string url);
                logger.LogInformation("Публичный URL: {url}", url);
            }
            catch (Exception ex) { logger.LogCritical(ex, string.Empty); }

            bool servicesOk = mySqlOk && mongoDbOk && botOk && urlOk;
            if (servicesOk is true)
            {
                logger.LogInformation("Все сервисы работают.");
                return servicesOk;
            }
            else
            {
                logger.LogCritical("Не все сервисы работают.\nMySql: {mySqlOk},\nMongoDB: {mongoDbOk},\nTelegramBot: {botOk},\nURL Getter: {urlOk}.", mySqlOk, mongoDbOk, botOk, urlOk);
                return servicesOk;
            }
        }
        #endregion


        #region Приватные методы.
        private static async Task<bool> EnsureMySqlAsync(IServiceProvider serviceProvider, int cancellationTokenIntervalSec = 5)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(cancellationTokenIntervalSec));

            var mySqlConnString = serviceProvider.GetConfiguration().GetRequiredSection("MySql").Value;
            var mySqlOptions = new DbContextOptionsBuilder<MySqlDbContext>().UseMySql(mySqlConnString, ServerVersion.AutoDetect(mySqlConnString)).Options;
            var mySql = new MySqlDbContext(mySqlOptions);
            return await mySql.Database.CanConnectAsync(cts.Token);
        }

        private static async Task<bool> EnsureMongoDbAsync(IServiceProvider serviceProvider, int cancellationTokenIntervalSec = 5)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(cancellationTokenIntervalSec));

            var mongoDb = serviceProvider.GetMongoDb();
            var dbNamesCoursor = await mongoDb.Client.ListDatabaseNamesAsync(cts.Token);
            var dbNamesList = await dbNamesCoursor.ToListAsync(cts.Token);

            return dbNamesList.Contains("vocabulary-improver");
        }

        private static async Task<bool> EnsureTelegramBotAsync(IServiceProvider serviceProvider, int cancellationTokenIntervalSec = 5)
        {
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(cancellationTokenIntervalSec));

            var bot = serviceProvider.GetBotClient();
            var name = (await bot.GetMyNameAsync(cancellationToken: cts.Token)).Name;
            return string.IsNullOrWhiteSpace(name) is false;
        }
        private static bool EnsureUrlGetted(IServiceProvider serviceProvider, out string url)
        {
            var urlGetter = serviceProvider.GetRequiredService<IUrlGetter>();
            url = urlGetter.GetUrl();
            return string.IsNullOrWhiteSpace(url) is false;
            
        }
        #endregion
    }
}
