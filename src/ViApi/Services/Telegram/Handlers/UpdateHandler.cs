using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using ViApi.Services.Repository;
using ViApi.Services.Telegram.Handlers.PartialsMessageHandlers;
using ViApi.Types.Telegram;

namespace ViApi.Services.Telegram.UpdateHandlers;


public class UpdateHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<UpdateHandler> _logger;
    private readonly IRepository _repository;
    private TelegramSession _session = null!;

    public UpdateHandler(ITelegramBotClient botClient, ILogger<UpdateHandler> logger, IRepository repository)
    {
        _botClient = botClient;
        _logger = logger;
        _repository = repository;
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
        _session = await _repository.GetOrRegisterSessionAsync(update.GetId(), update.GetFirstname(), cancellationToken);
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

        var msgHandlers = new MessageHandler(messageText, _session, repository: _repository, _botClient, cancellationToken);

        await msgHandlers.HandleMessageAsync();

    }
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        if (callbackQuery.Data is not { } callbackQueryMessage)
            return;

        _logger.LogInformation("Получен колбэкквэри от пользователя {tgid}, содержимое {messagetext}", _session.TelegramId, callbackQuery.Data);

        var msgHandlers = new MessageHandler(message: callbackQueryMessage, session: _session, callbackQueryId: callbackQuery.Id, repository: _repository, _botClient, cancellationToken);

        await msgHandlers.HandleMessageAsync();
    }
    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}
