using System.Net.Http.Json;
using ViTelegramBot.Http.JsonEntites;
using ViTelegramBot.Entities;
using Microsoft.Extensions.DependencyInjection;

namespace ViTelegramBot.Http;

public class ViApiHttpClient
{
    #region Dependency Injection and HttpClient
    private ServiceCollection services = new ServiceCollection();
    private ServiceProvider serviceProvider = null!;
    IHttpClientFactory httpClientFactory = null!;
    private HttpClient httpClient = null!;

    private void InitalizeDIHttpFactory()
    {
        services.AddHttpClient();
        serviceProvider = services.BuildServiceProvider();
        httpClientFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        httpClient = httpClientFactory.CreateClient();
    } 
    #endregion

    private readonly string hostname;

    public ViApiHttpClient(string hostname)
    {
        InitalizeDIHttpFactory();
        this.hostname = hostname;
        httpClient.GetAsync($"{hostname}/").Result.EnsureSuccessStatusCode();
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
                return new ViResult<string>(ViResultTypes.NotFounded, apiResponse.ResultValue, methodName, apiResponse.Message);
            }
        }
        return new ViResult<string>(ViResultTypes.NotFounded, null, methodName, $"Jwt for chatId: {id} was not recieved from API.");
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
    public async Task<ViResult<List<ViDictionary>>> GetDictsFromApiAsync(string jwt)
    {
        string methodName = nameof(GetDictsFromApiAsync);
        string url = $"{hostname}/api/dicts/get";

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        using var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            List<ViDictionary>? dict = await response.Content.ReadFromJsonAsync<List<ViDictionary>>();
            if (dict is not null)
            {
                return new ViResult<List<ViDictionary>>(ViResultTypes.Founded, dict, methodName, $"Dicts found, capacity: {dict.Count}.");
            }
        }
        else
        {
            ApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
            if (apiResponse is not null)
            {
                return new ViResult<List<ViDictionary>>(ViResultTypes.NotFounded, null, methodName, apiResponse.Message);
            }
        }
        return new ViResult<List<ViDictionary>>(ViResultTypes.NotFounded, null, methodName, $"Do dicts recieved.");
    }
    public async Task<ViResult<Guid>> AddNewDictToApiAsync(string jwt, string dictName)
    {
        string methodName = nameof(AddNewDictToApiAsync);
        string url = $"{hostname}/api/dicts/add/{dictName}";

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        using var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            ViDictionary? dict = await response.Content.ReadFromJsonAsync<ViDictionary>();
            if (dict is not null && dict.DictGuid != Guid.Empty)
            {
                return new ViResult<Guid>(ViResultTypes.Created, dict.DictGuid, methodName, $"Dict with name {dictName} created. Guid is {dict.DictGuid}.");
            }
        }
        
        return new ViResult<Guid>(ViResultTypes.Fail, Guid.Empty, methodName, $"Dict doesnt created. Http status code: {response.StatusCode}");
    }
}
