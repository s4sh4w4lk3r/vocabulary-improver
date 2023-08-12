using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using Throw;
using ViTelegramBot.ApiRequesting;
using ViTelegramBot.Entities;
using ViTelegramBot.Http.JsonEntites;

namespace ViTelegramBot.ViBot;

public static class UpdateHandlers
{
    private static ReplyKeyboardMarkup GetDefaultKeyboard()
    {
        var buttons = new List<KeyboardButton[]>()
        {
            new KeyboardButton[] { "Выбрать словарь", "Добавить новый словарь" },
        };

        ReplyKeyboardMarkup replyKeyboardMarkup = new(buttons)
        {
            ResizeKeyboard = true
        };

        return replyKeyboardMarkup;
    }

    private static ReplyKeyboardMarkup GetDictKeyboard()
    {
        var buttons = new List<KeyboardButton[]>()
        {
            new KeyboardButton[] { "Жобавить слова", "Удалить слова", "Переименовать словарь", "Удалить словарь" },
        };

        ReplyKeyboardMarkup replyKeyboardMarkup = new(buttons)
        {
            ResizeKeyboard = true
        };

        return replyKeyboardMarkup;
    }
    public static async Task OnStartAsync(ServiceProvider serviceProvider, ViApiClient viApi, ViSession userSession, Update update)
    {
        ChatId chatId = new(update.Message!.Chat.Id);
        ITelegramBotClient botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
        string helloMessage = string.Empty;
        ViSessionList viSessions = serviceProvider.GetRequiredService<ViSessionList>();

        var result = await viApi.SignUpUserAsync(chatId.Identifier.GetValueOrDefault(), update.Message!.Chat.FirstName!);
        if (result.ResultCode is ViResultTypes.Created)
        {
            helloMessage = "Вы зарегистрировались!";
        }
        if (result.ResultCode == ViResultTypes.Founded)
        {
            helloMessage = "Снова здраствуйте!";
        }
  
        var sendHelloTask = botClient.SendTextMessageAsync(
                chatId: chatId,
                text: helloMessage, 
                replyMarkup: GetDefaultKeyboard());

        var getMyDictsTask = GetMyDicts(serviceProvider, viApi, update);

        viSessions.UpdateState(userSession, UserState.Default);

        await Task.WhenAll(sendHelloTask, getMyDictsTask);
    }
    public static async Task GetMyDicts(ServiceProvider serviceProvider, ViApiClient viApi, Update update)
    {
        long chatId64 = update.Message!.Chat.Id;
        ChatId chatId = new(chatId64);
        var getDictsTask = viApi.GetDictList(chatId64);
        ITelegramBotClient botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
        var getDictResult = await getDictsTask;


        if (getDictResult.ResultCode is ViResultTypes.Founded && getDictResult.ResultValue is not null) 
        {
            string message = "Список ваших словарей:";
            int dictIndex = 1;
            foreach (var item in getDictResult.ResultValue)
            {
                message += $"\n{dictIndex}.{item.Name}";
            }

            await botClient.SendTextMessageAsync(
               chatId: chatId,
               text: message);
        }
        else
        {
            await botClient.SendTextMessageAsync(
               chatId: chatId,
               text: "У вас нет словарей.",
               replyMarkup: GetDefaultKeyboard());
        }
    }
    public static async Task AddNewWord(ServiceProvider serviceProvider, ViApiClient viApi, Update update, ViSession userSession, string messageText)
    {
        ITelegramBotClient botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
        long chatId64 = update.Message!.Chat.Id;
        ChatId chatId = new ChatId(update.Message!.Chat.Id);
        ViSessionList viSessions = serviceProvider.GetRequiredService<ViSessionList>();
        string[] words = messageText.Split(':');
        string sourceWord = string.Empty;
        string targetWord = string.Empty;
        try
        {
            sourceWord = words[0];
            targetWord = words[1];
        }
        catch (Exception) { }

        if (string.IsNullOrWhiteSpace(sourceWord) || string.IsNullOrWhiteSpace(targetWord))
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Неправильный ввод. Вводите в формате \"оригинал:перевод\".",
                replyMarkup: GetDefaultKeyboard());
            viSessions.UpdateState(userSession, UserState.Default);

            return;
        }

        var addNewDictResult = await viApi.AddNewWord(chatId64, userSession.SelectedDictionaryGuid, sourceWord, targetWord);
        if (addNewDictResult.ResultCode is ViResultTypes.Founded && addNewDictResult.ResultValue is true) 
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Слово добавлено",
                replyMarkup: GetDefaultKeyboard());

            userSession.SelectedDictionaryGuid = userSession.SelectedDictionaryGuid;
            viSessions.UpdateState(userSession, UserState.DictSelected);
        }
    }
    public static async Task ChooseDict(ServiceProvider serviceProvider, ViApiClient viApi, Update update, ViSession userSession, string messageText)
    {
        if (userSession.State is not UserState.ChoosingDict) { return; }

        long chatId64 = update.Message!.Chat.Id;
        var getDictsTask = viApi.GetDictList(chatId64);
        var botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
        var chatID = new ChatId(update.Message!.Chat.Id);
        var viSessionsList = serviceProvider.GetRequiredService<ViSessionList>();
        int dictId;
        if (int.TryParse(messageText, out dictId) is false)
        {
            await botClient.SendTextMessageAsync(chatID, "Число не распознано.");
            return;
        }

        var dictsList = await getDictsTask;
        var selectedDict = dictsList?.ResultValue?.ElementAtOrDefault(dictId - 1);

        if (selectedDict is null)
        {
            await botClient.SendTextMessageAsync(chatID, "Словарь не выбран.");
            return;
        }

        var getDictWordsTask = viApi.GetWordsAsync(chatId64, selectedDict.DictGuid);

        viSessionsList.UpdateState(userSession, UserState.DictSelected);
        viSessionsList.UpdateSelectedDictGuid(userSession, selectedDict.DictGuid);

        var wordsResult = await getDictWordsTask;
        var words = wordsResult.ResultValue;

        if (words is null || words.Count == 0) 
        {
            await botClient.SendTextMessageAsync(chatID, "Словарь пустой.");
            return;
        }
        else
        {
            messageText = string.Empty;
            foreach (var word in words)
            {
                messageText += $"{word.SourceWord} - {word.TargetWord} - {word.Rating}\n";
            }
            await botClient.SendTextMessageAsync(chatID, messageText);
            return;
        }

    }
}
