using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using ViTelegramBot.ApiRequesting;
using ViTelegramBot.Entities;
using ViTelegramBot.Http.JsonEntites;

namespace ViTelegramBot.ViBot;

public static class UpdateHandlers
{
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

        viSessions.UpdateState(userSession, UserState.Default);
        viSessions.UpdateSelectedDictGuid(userSession, Guid.Empty);
        viSessions.UpdateSelectedWordGuid(userSession, Guid.Empty);

        await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: helloMessage,
                replyMarkup: KeyboardSet.GetDefaultKeyboard());
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
               replyMarkup: KeyboardSet.GetDefaultKeyboard());
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
                replyMarkup: KeyboardSet.GetDefaultKeyboard());
            viSessions.UpdateState(userSession, UserState.Default);

            return;
        }

        var addNewDictResult = await viApi.AddNewWord(chatId64, userSession.SelectedDictionaryGuid, sourceWord, targetWord);
        if (addNewDictResult.ResultCode is ViResultTypes.Founded && addNewDictResult.ResultValue is true)
        {
            await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Слово добавлено",
                replyMarkup: KeyboardSet.GetDefaultKeyboard());

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
            await botClient.SendTextMessageAsync(chatID, "Словарь пустой.", replyMarkup: KeyboardSet.GetDictKeyboard());
            return;
        }
        else
        {
            messageText = $" Словарь: {selectedDict.Name!}\n\n";
            foreach (var word in words)
            {
                messageText += $"{word.SourceWord} - {word.TargetWord} - {word.Rating}\n";
            }
            await botClient.SendTextMessageAsync(chatID, messageText, replyMarkup: KeyboardSet.GetDictKeyboard());
            return;
        }
    }
    public static async Task WhenDictSelected(ServiceProvider serviceProvider, ViApiClient viApi, Update update, ViSession userSession, string messageText)
    {
        if (userSession.State is not UserState.DictSelected) { return; }

        var bot = serviceProvider.GetRequiredService<ITelegramBotClient>();
        var viSessionsList = serviceProvider.GetRequiredService<ViSessionList>();
        var chatID = new ChatId(update.Message!.Chat.Id);

        switch (messageText)
        {
            case "Добавить слово":
                viSessionsList.UpdateState(userSession, UserState.AddingWord);
                await bot.SendTextMessageAsync(chatID, text: "Добавьте слово, отправив \"оригинал - перевод\".");
                break;

            case "Удалить слово":
                viSessionsList.UpdateState(userSession, UserState.DeletingWord);
                await bot.SendTextMessageAsync(chatID, text: "Напишите номер слова, которое надо удалить.");
                break;

            case "Переименовать словарь":
                viSessionsList.UpdateState(userSession, UserState.RenamingDict);
                await bot.SendTextMessageAsync(chatID, text: "Напишите новое имя словаря.");
                break;

            case "Удалить словарь":
                await viApi.RemoveDictionaryAsync(update.Message!.Chat.Id, userSession.SelectedDictionaryGuid);
                await bot.SendTextMessageAsync(chatID, text: "Словарь удален.", replyMarkup: KeyboardSet.GetDefaultKeyboard());
                viSessionsList.UpdateState(userSession, UserState.Default);
                viSessionsList.UpdateSelectedDictGuid(userSession, Guid.Empty);
                break;

            case "Угадай-ка":

                break;
        }
    }
    private static Word RecognizeWord(ViSession userSession, ViApiClient viApi, int wordId)
    {
        throw new NotImplementedException();
    }
    public static async Task RenameDict(ServiceProvider serviceProvider, ViApiClient viApi, Update update, ViSession userSession, string newName)
    {
        if (userSession.State is not UserState.RenamingDict) { return; }
        if (string.IsNullOrWhiteSpace(newName)) { return; }

        var bot = serviceProvider.GetRequiredService<ITelegramBotClient>();
        var viSessionsList = serviceProvider.GetRequiredService<ViSessionList>();
        var chatID = new ChatId(update.Message!.Chat.Id);

        await viApi.UpdateDictNameAsync(update.Message!.Chat.Id, userSession.SelectedDictionaryGuid, newName);
        await bot.SendTextMessageAsync(chatID, "Имя словаря изменено.", replyMarkup: KeyboardSet.GetDictKeyboard());
        viSessionsList.UpdateState(userSession, UserState.DictSelected);

    }
}
