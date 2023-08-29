using MongoDB.Driver;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
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

        Message sentMessage = await msgHandlers.BotOnMessageReceived();

        _logger.LogInformation("The message was sent with id: {SentMessageId}", sentMessage.MessageId);

    }
    private async Task BotOnCallbackQueryReceived(CallbackQuery callbackQuery, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received inline keyboard callback from: {CallbackQueryId}", callbackQuery.Id);

        await _botClient.AnswerCallbackQueryAsync(
            callbackQueryId: callbackQuery.Id,
            text: $"Received {callbackQuery.Data}",
            cancellationToken: cancellationToken);

        await _botClient.SendTextMessageAsync(
            chatId: callbackQuery.Message!.Chat.Id,
            text: $"Received {callbackQuery.Data}",
            cancellationToken: cancellationToken);
    }
    private Task UnknownUpdateHandlerAsync(Update update, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Unknown update type: {UpdateType}", update.Type);
        return Task.CompletedTask;
    }
}
