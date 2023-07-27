using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using ViTelegramBot.ApiRequesting;
using ViTelegramBot.Entities;

namespace ViTelegramBot.ViBot;

internal class ViBot
{
    private string url = null!;
    private string secretToken;
    private TelegramBotClient botClient;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private HttpListener httpListener = new HttpListener();
    private ServiceProvider serviceProvider;
    private ViApiClient viApi;

    public ViBot(ServiceProvider serviceProvider, string ngrokToken, string botToken, string secretToken, ViApiClient viApi)
    {
        this.serviceProvider = serviceProvider;
        GetWebhookUrl(ngrokToken).Wait();
        botClient = new TelegramBotClient(botToken);
        this.secretToken = secretToken;
        httpListener.Prefixes.Add("http://127.0.0.1:8443/bot/");
        this.viApi = viApi;
    }

    public async Task StartAsync()
    {
        httpListener.Start();
        List < UpdateType> updates = new() { UpdateType.Message };
        await botClient.SetWebhookAsync(
            url: $"{url}/bot",
            allowedUpdates: updates,
            secretToken: secretToken,
            cancellationToken: cancellationTokenSource.Token);
        var me = await botClient.GetMeAsync();
        await Console.Out.WriteLineAsync(me.Username);
        GetUpdatesFromTgApiAsync();
    }

    public async Task StopAsync()
    {
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationTokenSource.Token, dropPendingUpdates: true);
        httpListener.Close();
        cancellationTokenSource.Cancel();
    }
    private async Task GetWebhookUrl(string ngrokToken)
    {
        IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        HttpClient httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{ngrokToken}");
        httpClient.DefaultRequestHeaders.Add("Ngrok-Version", "2");
        using var response = await httpClient.GetAsync("https://api.ngrok.com/tunnels");
        response.EnsureSuccessStatusCode();
        NgrokApiResponse? ngrokApiResponse = await response.Content.ReadFromJsonAsync<NgrokApiResponse>();
        if (ngrokApiResponse?.tunnels is null || ngrokApiResponse?.tunnels.Count == 0) { throw new InvalidOperationException("Empty ngrok tunnel."); }
        this.url = ngrokApiResponse!.tunnels!.FirstOrDefault()!.public_url!;
    }
    private async void GetUpdatesFromTgApiAsync()
    {
        while (cancellationTokenSource.IsCancellationRequested is false && httpListener.IsListening is true)
        { 
            var context = httpListener.GetContext();
            var insputstream = context.Request.InputStream;
            string json = new StreamReader(insputstream).ReadToEnd();
            Update update = JsonConvert.DeserializeObject<Update>(json)!;
            string updateInfo = $"UpdateId: {update.Id}, Type: {update.Type}";
            _ = Console.Out.WriteLineAsync(updateInfo);
            context.Response.StatusCode = 200;
            context.Response.Close();
            await UpdateHandler(update);
        }
    }

    private async Task UpdateHandler(Update update)
    {
        ChatId chatId = new(update.Message!.Chat.Id);
        string messageText = update.Message!.Text!;
        
        if (messageText == "/start") 
        { 
            var result = await viApi.SignUpUserAsync(chatId.Identifier.GetValueOrDefault(), update.Message!.Chat.FirstName!); 
            if (result.ResultCode is ViResultTypes.Created)
            {
                await botClient.SendTextMessageAsync(chatId, "Вы зарегистировались!");
            }

            if (result.ResultCode == ViResultTypes.Founded)
            {
                await botClient.SendTextMessageAsync(chatId, "Снова здраствуйте!");
            }
        }
    }

}
