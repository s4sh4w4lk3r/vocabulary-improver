using System.Net.Http.Json;
using ViTelegramBot.Http.JsonEntites;
using ViTelegramBot.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace ViTelegramBot.ApiRequesting;

public partial class ViApiHttpClient
{

    HttpClient httpClient = null!;
    private readonly string hostname;

    public ViApiHttpClient(string hostname, ServiceProvider serviceProvider)
    {
        InitalizeDIHttpFactory(serviceProvider);
        this.hostname = hostname;
        httpClient.GetAsync($"{hostname}/").Result.EnsureSuccessStatusCode();
    }

    private void InitalizeDIHttpFactory(ServiceProvider serviceProvider)
    {
        IHttpClientFactory httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        httpClient = httpClientFactory.CreateClient();
    }

    public async Task<ViResult<string>> GetJwtFromApiAsync(long id)
    {
        string methodName = nameof(GetJwtFromApiAsync);

        string url = $"{hostname}/api/auth/login/tg/{id}";

        using var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            Jwt? jwt = await response.Content.ReadFromJsonAsync<Jwt>();
            if (jwt is not null && jwt.JwtToken is not null) 
            {
                return new ViResult<string>(ViResultTypes.Founded, jwt.JwtToken, methodName, $"Jwt for chatId: {id} was recieved from API.");
            }
        }
        else
        {
            ApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
            if (apiResponse is not null)
            { 
                return new ViResult<string>(ViResultTypes.Fail, apiResponse.ResultValue, methodName, apiResponse.Message);
            }
        }
        return new ViResult<string>(ViResultTypes.Fail, null, methodName, $"Jwt for chatId: {id} was not recieved from API.");
    }
    public async Task<ViResult<string>> AddUserToApi(long id, string firstname)
    {
        string methodName = nameof(AddUserToApi);

        string url = $"{hostname}/api/auth/register/tg";

        using var response = await httpClient.PostAsJsonAsync(url, new { telegramid = id.ToString(), firstname = firstname });
        
        if (response.IsSuccessStatusCode)
        {
            return new ViResult<string>(ViResultTypes.Created, null, methodName, $"Jwt for chatId: {id} added to database.");
        }
        else
        {
            ApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
            if (apiResponse is not null)
            {
                return new ViResult<string>(ViResultTypes.Fail, apiResponse.ResultValue, methodName, apiResponse.Message);
            }
        }
        return new ViResult<string>(ViResultTypes.Fail, null, methodName, $"User not added in db.");
    }
}
