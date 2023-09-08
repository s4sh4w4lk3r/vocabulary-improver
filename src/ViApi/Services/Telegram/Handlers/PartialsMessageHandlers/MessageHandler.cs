using Serilog;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using ViApi.Services.Repository;
using ViApi.Types.Enums;
using ViApi.Types.Telegram;

namespace ViApi.Services.Telegram.Handlers.PartialsMessageHandlers;

public partial class MessageHandler
{
    private readonly IRepository _repository;
    private readonly ITelegramBotClient _botClient;
    private readonly CancellationToken _cancellationToken;
    private readonly string _recievedMessage;
    private readonly TelegramSession _session;
    private readonly string? _callbackQueryId;

    public MessageHandler(string message, TelegramSession session, IRepository repository, ITelegramBotClient botClient, CancellationToken cancellationToken = default)
    {
        _recievedMessage = message;
        _repository = repository;
        _botClient = botClient;
        _session = session;
        _cancellationToken = cancellationToken;
    }
    public MessageHandler(string message, TelegramSession session, string callbackQueryId, IRepository repository, ITelegramBotClient botClient, CancellationToken cancellationToken = default) :
        this(message, session, repository, botClient, cancellationToken)
    {
        _callbackQueryId = callbackQueryId;
    }

    public async Task HandleMessageAsync()
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
        _session.GameQueue?.Clear();
        var mongoTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
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

        var words = await _repository.GetWordsAsync(_session.UserGuid, _session.DictionaryGuid, _cancellationToken);
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
        var dicts = await _repository.GetDicionariesList(_session.UserGuid);
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
                await _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                return;
            }
            catch (ApiRequestException) { }
            return;
        }
        else
        {
            var msg = await _botClient.SendTextMessageAsync(_session.TelegramId, "Список ваших словарей:", replyMarkup: buttons, cancellationToken: _cancellationToken);

            _session.MessageIdToEdit = msg.MessageId;
            await _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);

            return;
        }

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
            UserState.AddingWord => AddNewWordAsync(),
            UserState.RenamingDict => RenameDictAsync(),
            UserState.DeletingWord => DeleteWordAsync(),
            UserState.AddingWordList => AddWordListAsync(),
            _ => _botClient.SendTextMessageAsync(_session.TelegramId, "Команда не найдена", cancellationToken: _cancellationToken)
        }; 

        await actionOnState;
    }
    private async Task DeleteUserAsync()
    {
        if (await _repository.DeleteUserAndSessionAsync(_session, _cancellationToken))
        {
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

            if (await _repository.CheckDictionaryIsExistAsync(_session.UserGuid, recievedGuid, _cancellationToken))
            {
                _session.State = UserState.DictSelected;
                _session.DictionaryGuid = recievedGuid;
                var getFormattedWordListTask = GetFormattedWordListAsync();
                var mongoInsertTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);

                var dictKeyboard = KeyboardSet.GetDictKeyboard(_session.DictionaryGuid);
                string messageWithWords = await getFormattedWordListTask;

                var editMessageTask = _botClient.EditMessageTextAsync(_session.TelegramId, _session.MessageIdToEdit, messageWithWords, replyMarkup: dictKeyboard, cancellationToken: _cancellationToken);
                //Приходится методы EditMessageTextAsync ставить в трай/кетч, так как если юзер нажмет на кнопку два раза быстро, недождавшись ответа, вылезет эксепшон, то что сообщение уже отредактировано текстом из первого запроса,
                // и оно не будет обновлено снова
                try 
                { 
                    await editMessageTask; 
                } 
                catch (ApiRequestException) { }

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

            if (mas.Length == 2 && Guid.TryParse(guidStr, out recievedGuid) is true && string.IsNullOrWhiteSpace(action) is false)
            {
                switch (action)
                {
                    case "addword":
                        {
                            _session.State = UserState.AddingWord;
                            _session.DictionaryGuid = recievedGuid;
                            var inserTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                            var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Введите сочетание слов в формате \"оригинал:перевод\"");
                            if (_callbackQueryId is not null) { await _botClient.AnswerCallbackQueryAsync(_callbackQueryId, cancellationToken: _cancellationToken); }
                            await Task.WhenAll(inserTask, sendMessageTask);
                            return true;
                        }

                    case "deleteword":
                        {
                            _session.State = UserState.DeletingWord;
                            _session.DictionaryGuid = recievedGuid;
                            var inserTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                            var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Введите номер слова для удаления", cancellationToken: _cancellationToken);
                            if (_callbackQueryId is not null) { await _botClient.AnswerCallbackQueryAsync(_callbackQueryId, cancellationToken: _cancellationToken); }
                            await Task.WhenAll(inserTask, sendMessageTask);
                            return true;
                        }

                    case "renamedict":
                        {
                            _session.State = UserState.RenamingDict;
                            _session.DictionaryGuid = recievedGuid;
                            var inserTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                            var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Введите новое имя для словаря");
                            if (_callbackQueryId is not null) { await _botClient.AnswerCallbackQueryAsync(_callbackQueryId, cancellationToken: _cancellationToken); }
                            await Task.WhenAll(inserTask, sendMessageTask);
                            return true;
                        }
                    case "deletedict":
                        {
                            _session.State = UserState.Default;
                            _session.DictionaryGuid = default;
                            var inserTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                            var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Словарь удален", cancellationToken: _cancellationToken);
                            var deleteDictTask = _repository.DeleteDictionaryAsync(_session.UserGuid, recievedGuid, _cancellationToken);
                            if (_callbackQueryId is not null) { await _botClient.AnswerCallbackQueryAsync(_callbackQueryId, cancellationToken: _cancellationToken); }

                            await Task.WhenAll(inserTask, sendMessageTask, deleteDictTask);
                            return true;
                        }
                    case "play":
                        if (_callbackQueryId is not null) { await _botClient.AnswerCallbackQueryAsync(_callbackQueryId, cancellationToken: _cancellationToken); }
                        if (_session.DictionaryGuid == default) { return true; }
                        await PlayAsync(recievedGuid, startNewGame: true);
                        return true;

                    case "addwordlist":
                        {
                            _session.State = UserState.AddingWordList;
                            _session.DictionaryGuid = recievedGuid;
                            var inserTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
                            var sendMessageTask = _botClient.SendTextMessageAsync(_session.TelegramId, "Введите словосочетания, начиная каждое с новой строки, разделяя двоеточием, например:" +
                                "\nоригинал1:перевод1\nоригинал2:перевод2");
                            if (_callbackQueryId is not null) { await _botClient.AnswerCallbackQueryAsync(_callbackQueryId, cancellationToken: _cancellationToken); }
                            await Task.WhenAll(inserTask, sendMessageTask);
                            return true;
                        }
                    case "answerword":
                        if (_callbackQueryId is not null) { await _botClient.AnswerCallbackQueryAsync(_callbackQueryId, cancellationToken: _cancellationToken); }
                        if ((_session.State is not UserState.Playing) || (_session.DictionaryGuid == default)) { return true; }
                        await PlayAsync(recievedGuid, startNewGame: false);
                        return true;
                }
            }
        }
        catch (IndexOutOfRangeException) { }

        return false;
    }
    private async Task GoBackToDictListAsync()
    {
        _session.State = UserState.Default;
        _session.DictionaryGuid = default;
        var inserTask = _repository.InsertOrUpdateUserSessionAsync(_session, _cancellationToken);
        var sendDictsTask = SendDictionariesListAsync(_session.MessageIdToEdit);
        await Task.WhenAll(inserTask, sendDictsTask);
    }
}