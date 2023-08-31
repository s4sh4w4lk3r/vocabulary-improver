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
using ViApi.Types.Enums;
using ViApi.Types.Telegram;

namespace ViApi.Services.Telegram.UpdateHandlers;

public class MessageHandlers
{
    private readonly MySqlDbContext _mysql;
    private readonly ITelegramBotClient _botClient;
    private readonly CancellationToken _cancellationToken;
    private readonly string _recievedMessage;
    private readonly IMongoDatabase _mongo;
    private readonly TelegramSession _session;
    private readonly string? _callbackQueryId;

    public MessageHandlers(string message, TelegramSession session, MySqlDbContext mysql, IMongoDatabase mongo, ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        _recievedMessage = message;
        _mysql = mysql;
        _botClient = botClient;
        _session = session;
        _mongo = mongo;
        _cancellationToken = cancellationToken;
    }
    public MessageHandlers(string message, TelegramSession session, string callbackQueryId, MySqlDbContext mysql, IMongoDatabase mongo, ITelegramBotClient botClient, CancellationToken cancellationToken = default) :
        this (message, session, mysql, mongo, botClient, cancellationToken = default)
    {
        _callbackQueryId = callbackQueryId;
    }
    public async Task HandleMessage()
    {


        var message = _recievedMessage switch
        {
            "/start" => SendOnStartMessageAsync(),
            "/deleteme" => DeleteUserAsync(),
            "Выбрать словарь" => SendDictionariesListAsync(),
            "Добавить новый словарь" => AddNewDictinaryAsync(),
            "backtodictlist" => GoBackToDictListAsync(),
            _ => ChooseActionOnState()
        };

        await message;
    }
    private async Task<Message> SendOnStartMessageAsync()
    {
        _session.State = UserState.Default;
        _session.DictionaryGuid = default;
        var mongoTask = _mongo.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
        var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Как приятно начать все с чистого листа.", cancellationToken: _cancellationToken, replyMarkup: KeyboardSet.GetDefaultKeyboard());
        await mongoTask;
        Log.Information("Сессия {session} обнулена.", _session);
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
    //Если нужно отредачить сообщение, то нужно ввести айдишник этого сообщения
    private async Task SendDictionariesListAsync(int messageId = default)
    {
        var dicts = await _mysql.Dictionaries.Where(d => d.UserGuid == _session.UserGuid).ToListAsync(cancellationToken: _cancellationToken);
        if (dicts.Count == 0)
        {
            await _botClient.SendTextMessageAsync(_session.TelegramId, "Словарей нет.", cancellationToken: _cancellationToken);
            return;
        }

        var buttons = KeyboardSet.GetDictListAsKeyboardButtons(dicts);

        if (messageId != default)
        {
            try
            {
                var msg = await _botClient.EditMessageTextAsync(chatId: _session.TelegramId, text: "Список ваших словарей:", messageId: messageId, replyMarkup: buttons, cancellationToken: _cancellationToken);
                _session.MessageIdToEdit = msg.MessageId;
                await _mongo.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                return;
            }
            catch { }
            return;
        }
        else
        {
            var msg = await _botClient.SendTextMessageAsync(_session.TelegramId, "Список ваших словарей:", replyMarkup: buttons, cancellationToken: _cancellationToken);

            _session.MessageIdToEdit = msg.MessageId;
            await _mongo.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);

            return;
        }
        
    }
    private async Task AddNewDictinaryAsync()
    {
        if (_session.State is not UserState.AddingDict)
        {
            _session.State = UserState.AddingDict;
            _session.DictionaryGuid = default;
            var insertTask = _mongo.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
            var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Введите название для нового словаря:", cancellationToken: _cancellationToken);
            await Task.WhenAll(insertTask, sendMessageTask);
            return;
        }
        if (_session.State is UserState.AddingDict)
        {
            string newDictName = _recievedMessage;
            var newDict = new Dictionary(Guid.NewGuid(), newDictName, _session.UserGuid);
            bool dictInserted = await _mysql.InsertDictionaryAsync(newDict, _cancellationToken);

            if (dictInserted is true)
            {
                _session.State = UserState.Default;
                var insertTask = _mongo.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                var sendOkTask = _botClient.SendTextMessageAsync(_session.TelegramId, $"Словарь \"{newDictName}\" добавлен", cancellationToken: _cancellationToken);
                var sendDictListTask = SendDictionariesListAsync();
                await Task.WhenAll(insertTask, sendOkTask, sendDictListTask);
                return;
            }
            else
            {
                await _botClient.SendTextMessageAsync(_session.TelegramId, $"Словарь почему-то не добавился в базу данных", cancellationToken: _cancellationToken);
                return;
            }
        }
        await _botClient.SendTextMessageAsync(_session.TelegramId, $"Ошибка сессии", cancellationToken: _cancellationToken);
        return;
    }
    private async Task ChooseActionOnState()
    {
        bool actionIsGuidOk = await ActionOnGuidAsync();
        if (actionIsGuidOk is true)
        {
            return;
        }

        var actionOnState = _session.State switch
        {
            UserState.AddingDict => AddNewDictinaryAsync(),
            _ => _botClient.SendTextMessageAsync(_session.TelegramId, "Команда не найдена", cancellationToken: _cancellationToken)
        };

        await actionOnState;
    }
    private async Task DeleteUserAsync()
    {
        var user = await _mysql.Users.FirstOrDefaultAsync(u => u.Guid == _session.UserGuid, _cancellationToken);
        if (user is not null)
        {
            await _mongo.DeleteUserSessionAsync(_session, _cancellationToken);
            _mysql.Users.Remove(user);
            await _mysql.SaveChangesAsync();
            await _botClient.SendTextMessageAsync(_session.TelegramId, "Вся информация про тебя удалена из базы данных. Напиши /start, если захочешь вернуться.", cancellationToken: _cancellationToken, replyMarkup: new ReplyKeyboardRemove());
        }
    }
    //Метод вернет труе если получен гуид или дейтсвие+гуид
    private async Task<bool> ActionOnGuidAsync()
    {
        //Просто получение гуида, просто присланный гуид означает выбранный словарь. Вернет труе если отработал номрмально и работа вызывающего метода должна быть прекращена
        if (Guid.TryParse(_recievedMessage, out Guid recievedGuid) is true)
        {
            recievedGuid.Throw().IfDefault();

            if ((await _mysql.Dictionaries.Where(d => d.Guid == recievedGuid && d.UserGuid == _session.UserGuid).CountAsync()) == 1)
            {
                _session.State = UserState.DictSelected;
                _session.DictionaryGuid = recievedGuid;
                var getFormattedWordListTask = GetFormattedWordListAsync();
                var mongoInsertTask = _mongo.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);

                var dictKeyboard = KeyboardSet.GetDictKeyboard(_session.DictionaryGuid);
                string messageWithWords = await getFormattedWordListTask;

                var editMessageTask = _botClient.EditMessageTextAsync(_session.TelegramId, _session.MessageIdToEdit, messageWithWords, replyMarkup: dictKeyboard, cancellationToken: _cancellationToken);
                //Приходится методы EditMessageTextAsync ставить в трай/кетч, так как если юзер нажмет на кнопку два раза быстро, недождавшись ответа, вылезет эксепшон, то что сообщение уже отредактировано текстом из первого запроса,
                // и оно не будет обновлено снова
                try { await editMessageTask; } catch { }

                await mongoInsertTask;
                return true;
            }
        }

        //Действие + гуид
        try
        {
            var mas = _recievedMessage.Split(':');
            string action = mas[0];
            string guidStr = mas[1];

            if ((mas.Length == 2) && (Guid.TryParse(guidStr, out Guid recievedDictGuid) is true) && (string.IsNullOrWhiteSpace(action) is false))
            {
                switch (action)
                {
                    case "addword":
                        {
                            _session.State = UserState.AddingWord;
                            _session.DictionaryGuid = recievedDictGuid;
                            var inserTask = _mongo.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                            var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Введите сочетание слов в формате \"оригинал:перевод\"");
                            await Task.WhenAll(inserTask, sendMessageTask);
                            return true;
                        }

                    case "deleteword":
                        {
                            _session.State = UserState.DeletingWord;
                            _session.DictionaryGuid = recievedDictGuid;
                            var inserTask = _mongo.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                            var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Введите номер слова для удаления", cancellationToken: _cancellationToken);
                            await Task.WhenAll(inserTask, sendMessageTask);
                            return true;
                        }

                    case "renamedict":
                        {
                            _session.State = UserState.RenamingDict;
                            _session.DictionaryGuid = recievedDictGuid;
                            var inserTask = _mongo.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                            var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Введите новое имя для словаря");
                            await Task.WhenAll(inserTask, sendMessageTask);
                            return true;
                        }
                    case "deletedict":
                        {
                            _session.State = UserState.Default;
                            _session.DictionaryGuid = default;
                            var inserTask = _mongo.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                            var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Словарь удален", cancellationToken: _cancellationToken);
                            var deleteDictTask = _mysql.DeleteDictionaryAsync(_session.UserGuid, recievedDictGuid, _cancellationToken);
                            if (_callbackQueryId is not null) 
                            {
                                await _botClient.AnswerCallbackQueryAsync(_callbackQueryId, text: "Удалено", cancellationToken: _cancellationToken) ; 
                            }
                            
                            await Task.WhenAll(inserTask, sendMessageTask, deleteDictTask);
                            return true;
                        }
                    case "play":


                        break;
                }
            }
        }
        catch { }

        return false;
    }
    private async Task GoBackToDictListAsync()
    {
        _session.State = UserState.Default;
        _session.DictionaryGuid = default;
        var inserTask = _mongo.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
        var sendDictsTask = SendDictionariesListAsync(_session.MessageIdToEdit);
        await Task.WhenAll(inserTask, sendDictsTask);
    }
}