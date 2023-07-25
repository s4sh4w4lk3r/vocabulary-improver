using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using ViTelegramBot.Entities;
using ViTelegramBot.Http.JsonEntites;

namespace ViTelegramBot.ApiRequesting;

public partial class ViApiHttpClient
{
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
    public async Task<ViResult<Guid>> RemoveDictFromApiAsync(string jwt, Guid dictGuid)
    {
        string methodName = nameof(RemoveDictFromApiAsync);
        string url = $"{hostname}/api/dicts/remove/{dictGuid}";

        if (dictGuid == Guid.Empty)
        {
            return new ViResult<Guid>(ViResultTypes.Fail, Guid.Empty, methodName, "Entered guid is empty.");
        }

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        using var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            ViDictionary? dict = await response.Content.ReadFromJsonAsync<ViDictionary>();
            if (dict is not null && dict.DictGuid != Guid.Empty)
            {
                return new ViResult<Guid>(ViResultTypes.Removed, dict.DictGuid, methodName, $"Dict with guid {dict.DictGuid} removed.");
            }
        }
        else
        {
            ApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
            if (apiResponse is not null && apiResponse.Message is not null) 
            {
                return new ViResult<Guid>(ViResultTypes.Fail, Guid.Empty, methodName, apiResponse.Message);
            }
        }
        return new ViResult<Guid>(ViResultTypes.Fail, Guid.Empty, methodName, $"Dict doesnt removed. Http status code: {response.StatusCode}");
    }
    public async Task<ViResult<Guid>> UpdateDictNameApi(string jwt, Guid dictGuid, string newName)
    {
        string methodName = nameof(UpdateDictNameApi);
        string url = $"{hostname}/api/dicts/editname/{dictGuid}/{newName}";

        if (dictGuid == Guid.Empty)
        {
            return new ViResult<Guid>(ViResultTypes.Fail, Guid.Empty, methodName, "Entered guid is empty.");
        }

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        using var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            ViDictionary? dict = await response.Content.ReadFromJsonAsync<ViDictionary>();
            if (dict is not null && dict.DictGuid != Guid.Empty)
            {
                return new ViResult<Guid>(ViResultTypes.Updated, dict.DictGuid, methodName, $"Dict with guid {dict.DictGuid} updated.");
            }
        }
        else
        {
            ApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
            if (apiResponse is not null && apiResponse.Message is not null)
            {
                return new ViResult<Guid>(ViResultTypes.Fail, Guid.Empty, methodName, apiResponse.Message);
            }
        }
        return new ViResult<Guid>(ViResultTypes.Fail, Guid.Empty, methodName, $"Dict doesnt updated. Http status code: {response.StatusCode}");
    }
}
