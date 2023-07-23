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

    public async Task<ViResult<string>> GetJwtFromApiAsync(long id)
    {
        string methodName = nameof(GetJwtFromApiAsync);

        string url = $"{hostname}/api/auth/login/tg/{id}";

        try
        {
            Jwt? jwt = await httpClient.GetFromJsonAsync<Jwt>(url);

            if (jwt is not null && jwt.JwtToken is not null)
            {
                return new ViResult<string>(ViResultTypes.Founded, jwt.JwtToken, methodName, $"Jwt for chatId: {id} was recieved from API.");
            }
        }
        catch (Exception)
        {
            
        }

        return new ViResult<string>(ViResultTypes.NotFounded, null, methodName, $"Jwt for chatId: {id} was not recieved from API.");
    }
    public async Task<ViResult<string>> AddNewUser(long id, string firstname)
    {
        string methodName = nameof(AddNewUser);

        string url = $"{hostname}/api/auth/register/tg";

        using var response = await httpClient.PostAsJsonAsync(url, new { telegramid = id.ToString(), firstname = firstname });
        
        if (response.IsSuccessStatusCode)
        {
            return new ViResult<string>(ViResultTypes.Created, null, methodName, $"Jwt for chatId: {id} added to database.");
        }

        return new ViResult<string>(ViResultTypes.Fail, null, methodName, $"Server bad response, maybe user exists.");
    }
}
