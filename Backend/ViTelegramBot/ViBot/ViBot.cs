using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Throw;
using ViTelegramBot.ApiRequesting;
using ViTelegramBot.Entities;
using static ViTelegramBot.ViBot.UpdateHandlers;

namespace ViTelegramBot.ViBot;

internal class ViBot
{
    private ViSessionList ViSessions { get; set; }
    private HttpListener HttpListener { get; set; } = new HttpListener();
    private ServiceProvider ServiceProvider { get; set; }
    private ViApiClient ViApi { get; set; }
    private string Url { get; set; } = null!; //URL задается в методе GetWebhookUrl.
    private string SecretToken { get; set; }
    private ITelegramBotClient BotClient { get; set; }
    private CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

    public ViBot(ServiceProvider serviceProvider, ViApiClient viApi)
    {
        ServiceProvider = serviceProvider;
        string ngrokToken = serviceProvider.GetRequiredService<IConfiguration>().GetRequiredSection("ngrokToken").Value!;
        SecretToken = serviceProvider.GetRequiredService<IConfiguration>().GetRequiredSection("WebhookSecretToken").Value!;
        ViApi = viApi;
        BotClient = serviceProvider.GetRequiredService<ITelegramBotClient>();
        ViSessions = serviceProvider.GetRequiredService<ViSessionList>();

        GetWebhookUrl(ngrokToken).Wait();
        HttpListener.Prefixes.Add("http://127.0.0.1:8443/vibot/");
    }

    public void Start()
    {
        HttpListener.Start();
        SetWebHook();
        GettingUpdates();
    }

    public void Stop()
    {
        DeleteWebHook();
        CancellationTokenSource.Cancel();
    }
    #region WebHookSetting
    private void SetWebHook()
    {
        List<UpdateType> updates = new() { UpdateType.Message };
        BotClient.SetWebhookAsync(
            url: $"{Url}/vibot",
            allowedUpdates: updates,
            secretToken: SecretToken,
            cancellationToken: CancellationTokenSource.Token).Wait();
        var me = BotClient.GetMeAsync().Result;
        Console.WriteLine($"Webhook setted, Bot: {me.Username}");
    }
    private void DeleteWebHook()
    {
        BotClient.DeleteWebhookAsync(cancellationToken: CancellationTokenSource.Token, dropPendingUpdates: true).Wait();
        Console.WriteLine($"Webhook deleted.");
    }
    #endregion

    private async Task GetWebhookUrl(string ngrokToken)
    {
        IHttpClientFactory httpClientFactory = ServiceProvider.GetRequiredService<IHttpClientFactory>();
        HttpClient httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{ngrokToken}");
        httpClient.DefaultRequestHeaders.Add("Ngrok-Version", "2");
        using var response = await httpClient.GetAsync("https://api.ngrok.com/tunnels");
        response.EnsureSuccessStatusCode();
        NgrokApiResponse? ngrokApiResponse = await response.Content.ReadFromJsonAsync<NgrokApiResponse>();
        ngrokApiResponse.ThrowIfNull().IfCountEquals(t => t.tunnels!, 0, "Empty ngrok tunnel.");
        Url = ngrokApiResponse!.tunnels!.FirstOrDefault()!.public_url!;
    }
    private void GettingUpdates()
    {
        new Task(async () =>
        {
            while (true)
            {
                if (CancellationTokenSource.IsCancellationRequested is true)
                {
                    HttpListener.Stop();
                    return;
                }
                var context = HttpListener.GetContext();
                var insputstream = context.Request.InputStream;
                string json = new StreamReader(insputstream).ReadToEnd();
                Update update = JsonConvert.DeserializeObject<Update>(json)!;
                string updateInfo = $"UpdateId: {update.Id}, UserId: {update.Message?.Chat.Id}, Type: {update.Type}, Message: {update.Message?.Text}";
                _ = Console.Out.WriteLineAsync(updateInfo);
                context.Response.StatusCode = 200;
                context.Response.Close();
                await OnUpdateHandler(update);
            }
        }).Start();
    }
    private async Task OnUpdateHandler(Update update)
    {
        string messageText = update.Message!.Text!;
        long chatId = update.Message.Chat.Id;
        ViSession? userSession = ViSessions.FirstOrDefault(s => s.TelegramId == chatId);
        if (userSession is null)
        {
            await ViApi.SignUpUserAsync(chatId, update.Message.Chat.FirstName!);
            userSession = ViSessions.FirstOrDefault(s => s.TelegramId == chatId);
            if (userSession is null)
            {
                await Console.Out.WriteLineAsync("Что-то не так. Пользователя нет в сессиях.");
                return;
            }
        }

        if (messageText == "/start")
        {
            await OnStartAsync(ServiceProvider, ViApi, userSession, update);
            return;

        }
        if (messageText == "Выбрать словарь")
        {
            bool dictIsNotEmpty = await GetMyDicts(ServiceProvider, ViApi, update);
            if (dictIsNotEmpty is false) { return; }

            await BotClient.SendTextMessageAsync(chatId, "Введите номер словаря:");
            ViSessions.UpdateState(userSession, UserState.ChoosingDict);
            return;
        }
        if (messageText == "Добавить новый словарь")
        {
            ViSessions.UpdateState(userSession, UserState.AddingDict);
            await BotClient.SendTextMessageAsync(chatId, "Введите название словаря");
            return;
        }

        switch (userSession.State)
        {

            case UserState.Default:
                
                break;
            case UserState.ChoosingDict:
                await ChooseDict(ServiceProvider, ViApi, update, userSession, messageText);
                break;
            case UserState.ChoosingWord:
                break;
            case UserState.Playing:
                break;
            case UserState.AddingWord:
                await AddNewWord(ServiceProvider, ViApi, update, userSession, messageText);
                break;
            case UserState.AddingDict:
                await AddNewDict(ServiceProvider, ViApi, userSession, messageText);
                break;
            case UserState.DeletingWord:
                await DeleteWord(ServiceProvider, ViApi, update, userSession, messageText);
                break;
            case UserState.DictSelected:
                await WhenDictSelected(ServiceProvider, ViApi, update, userSession, messageText);
                break;
            case UserState.RenamingDict:
                await RenameDict(ServiceProvider, ViApi, update, userSession, messageText);
                break;
        }
    }
}