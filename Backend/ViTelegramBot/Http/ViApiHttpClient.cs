using System.Net.Http.Json;
using System;
using ViTelegramBot.Http.JsonEntites;
using ViTelegramBot.Entities;

namespace ViTelegramBot.Http;

public class ViApiHttpClient
{
    private readonly string hostname; 

    private static SocketsHttpHandler socketsHandler = new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(20)
    };
    private static HttpClient httpClient { get; } = new HttpClient(socketsHandler);


    public ViApiHttpClient(string hostname)
    {
        this.hostname = hostname;
        httpClient.GetAsync($"{hostname}/").Result.EnsureSuccessStatusCode();
    }

    public ViResult<string> GetJwtFromApi(long id)
    {
        string methodName = nameof(GetJwtFromApi);

        string url = $"{hostname}/api/auth/login/tg/{id}";

        var task = httpClient.GetFromJsonAsync<Jwt>(url);
        task.Wait();

        Jwt? jwt = task.Result;
        if (jwt is not null && jwt.JwtToken is not null)
        {
            return new ViResult<string>(ViResultTypes.Founded, jwt.JwtToken, methodName, $"Jwt for chatId: {id} was recieved from API.");
        }
        else
        {
            return new ViResult<string>(ViResultTypes.NotFounded, null, methodName, $"Jwt for chatId: {id} was not recieved from API.");
        }
    }
}
