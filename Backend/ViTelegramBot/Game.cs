using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using ViTelegramBot.ApiRequesting;
using ViTelegramBot.Entities;
using ViTelegramBot.Http.JsonEntites;

namespace ViTelegramBot;

public class Game
{
    private ViApiClient ViApiClient { get; set; }
    private Guid DictionaryGuid { get; set; }
    private ITelegramBotClient BotClient { get; set; }
    private List<Word> Words { get; set; } = null!;
    private ViSession UserSession { get; set; }

    event Action<string>? WordNotifier;

    public Game(IServiceProvider serviceProvider, ViApiClient viApiClient, ViSession userSession, out Action<string>? wordNotifier)
    {
        ViApiClient = viApiClient;
        DictionaryGuid = userSession.SelectedDictionaryGuid;
        BotClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
        UserSession = userSession;
        wordNotifier = WordNotifier;
    }

    public async Task Start(CancellationToken token)
    {
        var chatId = new ChatId(UserSession.TelegramId);
        Words = (await ViApiClient.GetWordsAsync(UserSession.TelegramId, DictionaryGuid)).ResultValue!;

        if (Words is null) 
        { 
            await BotClient.SendTextMessageAsync(chatId, "Произошла ошибка, слова есть нулл.");
            return; 
        }

    }
    private async Task AskWord(Word word)
    {

    }
}
