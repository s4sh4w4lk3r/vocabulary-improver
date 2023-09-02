using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using ViApi.Types.Configuration;

namespace ViApi.Services.Telegram
{
    public class ConfigureWebhook : IHostedService
    {
        private readonly ILogger<ConfigureWebhook> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly BotConfiguration _botConfig;

        public ConfigureWebhook(ILogger<ConfigureWebhook> logger, IServiceProvider serviceProvider, IOptions<BotConfiguration> botOptions)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _botConfig = botOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            var webhookAddress = $"{_botConfig.WebhookUrl}{"/bot"}";
            await botClient.SetWebhookAsync(
                url: webhookAddress,
                allowedUpdates: Array.Empty<UpdateType>(),
                secretToken: _botConfig.WebhookSecretToken,
                cancellationToken: cancellationToken);
            _logger.LogInformation("Вебхук установлен: {WebhookAddress}", webhookAddress);
        }
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var botClient = scope.ServiceProvider.GetRequiredService<ITelegramBotClient>();

            await botClient.DeleteWebhookAsync(cancellationToken: cancellationToken);
            _logger.LogInformation("Вебхук удален");
        }
    }
}
