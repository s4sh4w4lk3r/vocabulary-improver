using MongoDB.Driver;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using ViApi.Services.MySql;
using ViApi.Types.Telegram;

namespace ViApi.Services.Telegram.UpdateHandlers;


public class UpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly MySqlDbContext _mysql;
    private readonly IMongoDatabase _mongo;
    private TelegramSession _session = null!;

    public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, MySqlDbContext mysql, IMongoDatabase mongo)
    {
        _botClient = botClient;
        _logger = logger;
        _mysql = mysql;
        _mongo = mongo;
    }
    public Task HandleErrorAsync(Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.LogInformation("HandleError: {ErrorMessage}", ErrorMessage);
        return Task.CompletedTask;
    }
    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        _session = await new UserHandlers(update, _mysql, _mongo, cancellationToken).GetSessionAsync();
        var handler = update switch
        {
            { Message: { } message } => BotOnMessageReceived(message, cancellationToken),
            { CallbackQuery: { } callbackQuery } => BotOnCallbackQueryReceived(callbackQuery, cancellationToken),
            _ => UnknownUpdateHandlerAsync(update, cancellationToken)
        };

        await handler;
    }

    private async Task BotOnMessageReceived(Message message, CancellationToken cancellationToken)
    {
        if (message.Text is not { } messageText)
            return;

        _logger.LogInformation("Получено сообщение типа {MessageType} от пользователя {tgid}, содержимое {messagetext}", message.Type, _session.TelegramId, messageText);

        var msgHandlers = new MessageHandlers(messageText, _session, _mysql, _mongo, _botClient, cancellationToken);

        await msgHandlers.HandleMessage();

    }
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        if (callbackQuery.Data is not { } callbackQueryMessage)
            return;

        _logger.LogInformation("Получен колбэкквэри от пользователя {tgid}, содержимое {messagetext}", _session.TelegramId, callbackQuery.Data);

        var msgHandlers = new MessageHandlers(message: callbackQueryMessage, session: _session, callbackQueryId: callbackQuery.Id, _mysql, _mongo, _botClient, cancellationToken);

        await msgHandlers.HandleMessage();
    }
    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}
