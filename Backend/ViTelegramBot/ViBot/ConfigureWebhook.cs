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
using Telegram.Bot.Types.Enums;
using ViTelegramBot.Entities;

namespace ViTelegramBot.ViBot;

internal class ConfigureWebhook
{
    private string url;
    private string secretToken;
    private TelegramBotClient botClient;
    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    private HttpListener httpListener = new HttpListener();
    private ServiceProvider serviceProvider;

    public ConfigureWebhook(ServiceProvider serviceProvider, string ngrokToken, string botToken, string secretToken)
    {
        this.serviceProvider = serviceProvider;
        url = GetWebhookUrl(ngrokToken).Result;
        botClient = new TelegramBotClient(botToken);
        this.secretToken = secretToken;
        httpListener.Prefixes.Add("http://127.0.0.1:8443/bot/");
    }

    public async Task StartAsync()
    {
        httpListener.Start();
        
        await botClient.SetWebhookAsync(
            url: url,
            allowedUpdates: Array.Empty<UpdateType>(),
            secretToken: secretToken,
            cancellationToken: cancellationTokenSource.Token);
        var me = await botClient.GetMeAsync();
        await Console.Out.WriteLineAsync(me.Username);
    }

    public async Task StopAsync()
    {
        httpListener.Stop();
        await botClient.DeleteWebhookAsync(cancellationToken: cancellationTokenSource.Token);
    }
    private async Task<string> GetWebhookUrl(string ngrokToken)
    {
        IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        HttpClient httpClient = httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{ngrokToken}");
        httpClient.DefaultRequestHeaders.Add("Ngrok-Version", "2");
        using var response = await httpClient.GetAsync("https://api.ngrok.com/tunnels");
        response.EnsureSuccessStatusCode();
        NgrokApiResponse? ngrokApiResponse = await response.Content.ReadFromJsonAsync<NgrokApiResponse>();
        string url = ngrokApiResponse?.tunnels?.FirstOrDefault()?.public_url!;
        return url;
    }
}
