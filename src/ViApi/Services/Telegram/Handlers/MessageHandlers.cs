using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Serilog;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using ViApi.Services.MongoDb;
using ViApi.Services.MySql;
using ViApi.Types.Common;
using ViApi.Types.Telegram;

namespace ViApi.Services.Telegram.UpdateHandlers;

public class MessageHandlers
{
    private readonly MySqlDbContext _mysql;
    private readonly ITelegramBotClient _botClient;
    private readonly CancellationToken _cancellationToken;
    private readonly string _recievedMessage;
    private readonly IMongoDatabase _mongo;
    private TelegramSession _session;

    public MessageHandlers(string message, TelegramSession session, MySqlDbContext mysql, IMongoDatabase mongo, ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        _recievedMessage = message;
        _mysql = mysql;
        _botClient = botClient;
        _session = session;
        _mongo = mongo;
        _cancellationToken = cancellationToken;
    }
    public async Task<Message> BotOnMessageReceived()
    {
        var action = _recievedMessage switch
        {
            "/start" => SendOnStartMessageAsync(),
            "/deleteme" => _botClient.SendTextMessageAsync(_session.TelegramId, "NotImplementedException", cancellationToken: _cancellationToken),
            "Выбрать словарь" => SendDictionariesListAsync(),
            "Добавить новый словарь" => _botClient.SendTextMessageAsync(_session.TelegramId, "NotImplementedException", cancellationToken: _cancellationToken),
            _ => _botClient.SendTextMessageAsync(_session.TelegramId, "Команда не найдена.", cancellationToken: _cancellationToken)
        };

        return await action;
    }
    private async Task<Message> SendOnStartMessageAsync()
    {
        _session.State = Types.Enums.UserState.Default;
        _session.DictionaryGuid = default;
        var mongoTask = _mongo.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
        var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Как приятно начать все с чистого листа.", cancellationToken: _cancellationToken, replyMarkup: KeyboardSet.GetDefaultKeyboard());
        await mongoTask;
        Log.Information("Сессия {session} обнулена." , _session);
        return await sendMessageTask;
    }
    private async Task<string> GetFormattedWordListAsync()
    {
        if (_session.UserGuid == default || _session.DictionaryGuid == default)
        {
            return "Произошла внутренняя ошибка.";
        }

        var words = await _mysql.GetWordsAsync(_session.UserGuid, _session.DictionaryGuid, _cancellationToken);
        if (words is null || words.Count == 0)
        {
            return "Словарь пустой.";
        }

        var message = new StringBuilder();

        int number = 0;
        foreach (var word in words) 
        {
            number++;
            message.Append($"{number}. {word.SourceWord} - {word.TargetWord}\n");
        }
        message.Remove(message.Length - 1, 1);

        return message.ToString();
    }
    private async Task<Message> SendDictionariesListAsync()
    {
        var dicts = await _mysql.Dictionaries.Where(d => d.UserGuid == _session.UserGuid).ToListAsync(cancellationToken: _cancellationToken);
        if (dicts.Count == 0) 
        {  
            return await _botClient.SendTextMessageAsync(_session.TelegramId, "Словарей нет.", cancellationToken: _cancellationToken); 
        }

        var buttons = GetDictListAsKeyboardButtons(dicts);

        return await _botClient.SendTextMessageAsync(_session.TelegramId, "Список ваших словарей:", replyMarkup: buttons, cancellationToken: _cancellationToken);
    }
    private static InlineKeyboardMarkup GetDictListAsKeyboardButtons(IEnumerable<Dictionary> dictionaries)
    {
        var dictqueue = new Queue<Dictionary>(dictionaries);
        int dictCount = dictionaries.Count();
        int rows;
        bool IsCountEven = dictCount % 2 == 0;
        var buttonList = new List<IEnumerable<InlineKeyboardButton>>();
        
        if (IsCountEven)
        {
            rows = dictCount / 2;
        }
        else
        {
            rows = (dictCount / 2) + 1;
        }

        for (int i = 0; i < rows; i++)
        {
            var buttonPair = new List<InlineKeyboardButton>();

            bool dict1Dequeued = dictqueue.TryDequeue(out Dictionary dict1);
            bool dict2Dequeued = dictqueue.TryDequeue(out Dictionary dict2);

            if ((dict1Dequeued is true && dict2Dequeued is true) && ((dict1 is not null) && (dict2 is not null)))
            {
                buttonPair.Add(InlineKeyboardButton.WithCallbackData($"{dict1.Name}", $"{dict1.Guid}"));
                buttonPair.Add(InlineKeyboardButton.WithCallbackData($"{dict2.Name}", $"{dict2.Guid}"));
                buttonList.Add(buttonPair);
            }
            if ((dict1Dequeued is true && dict2Dequeued is false) && ((dict1 is not null) && (dict2 is null)))
            {
                buttonPair.Add(InlineKeyboardButton.WithCallbackData($"{dict1.Name}", $"{dict1.Guid}"));
                buttonList.Add(buttonPair);
            }
            else if ((dict1Dequeued is false && dict2Dequeued is false) && ((dict1 is null) && (dict2 is null)))
            {
                break;
            }
        }

        return new InlineKeyboardMarkup(buttonList);
    }
}
