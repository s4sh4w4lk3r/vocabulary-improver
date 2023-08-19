using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using NgrokApi;
using System.Threading;
using Telegram.Bot;
using Throw;
using ViApi.Database;

namespace ViApi.Extensions
{
    public static class ServiceProviderExtensions
    {
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
        public async static Task<string?> GetFirstTunnelUrlAsync(this Ngrok ngrok, CancellationToken cancellationToken)
        {
            var tunnel = await ngrok.Tunnels.List().FirstOrDefaultAsync(cancellationToken);
            return tunnel?.PublicUrl;
        }
        public async static Task<bool> EnsureServicesOkAsync(this IServiceProvider serviceProvider, ILogger logger)
        {
            bool mySqlOk = false;
            bool mongoDbOk = false;
            bool botOk = false;
            bool ngrokOk = false;

            string? ngrokUrl = null;


            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                var ngrok = serviceProvider.GetRequiredService<Ngrok>();
                var ngrokTask = ngrok.GetFirstTunnelUrlAsync(cts.Token);
                ngrokUrl = await ngrokTask;
                ngrokOk = string.IsNullOrWhiteSpace(ngrokUrl) is false;
                ngrokOk.Throw(_ => new Exception("Не получен Ngrok URL.")).IfFalse();
            }
            catch (Exception ex) { logger.LogCritical(ex, string.Empty); }


            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                var bot = serviceProvider.GetBotClient();
                var botTask = bot.GetMyNameAsync(cancellationToken: cts.Token);
                botOk = string.IsNullOrWhiteSpace((await botTask).Name) is false;
            }
            catch (Exception ex) { logger.LogCritical(ex, string.Empty); }


            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                var mySqlConnString = serviceProvider.GetConfiguration().GetRequiredSection("MySql").Value;
                var mySqlOptions = new DbContextOptionsBuilder<MySqlDbContext>().UseMySql(mySqlConnString, ServerVersion.AutoDetect(mySqlConnString)).Options;
                var mySql = new MySqlDbContext(mySqlOptions);
                var mySqlTask = mySql.Database.CanConnectAsync(cts.Token);
                mySqlOk = await mySqlTask;
            }
            catch (Exception ex) { logger.LogCritical(ex, string.Empty); }


            try
            {
                var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));

                var mongoDb = serviceProvider.GetMongoDb();
                var mongoDbTask = (await mongoDb.Client.ListDatabaseNamesAsync(cts.Token)).AnyAsync(cts.Token);
                mongoDbOk = await mongoDbTask;
            }
            catch (Exception ex) { logger.LogCritical(ex, string.Empty); }


            bool servicesOk = mySqlOk && mongoDbOk && ngrokOk && botOk;
            if (servicesOk is true)
            {
                logger.LogInformation("Все сервисы работают.");
                logger.LogInformation("Ngrok URL: {ngrokUrl}", ngrokUrl);
                return servicesOk;
            }
            else
            {
                logger.LogCritical("Не все сервисы работают.\nMySql: {mySqlOk},\nMongoDB: {mongoDbOk},\nNgrok: {ngrokOk},\nTelegramBot: {botOk}.", mySqlOk, mongoDbOk, ngrokOk, botOk);
                return servicesOk;
            }
        }
    }
}
