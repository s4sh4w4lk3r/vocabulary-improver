using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using ViTelegramBot.Entities;
using ViTelegramBot.Http.JsonEntites;

namespace ViTelegramBot.ApiRequesting;

public partial class ViApiHttpClient
{
    public async Task<ViResult<List<Word>>> GetWordsFromApiAsync(string jwt, Guid dictGuid)
    {
        string methodName = nameof(GetWordsFromApiAsync);
        string url = $"{hostname}/api/words/get/{dictGuid}";

        if (dictGuid == Guid.Empty)
        {
            return new ViResult<List<Word>>(ViResultTypes.Fail, null, methodName, "Entered guid is empty.");
        }

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        using var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            List<Word>? words = await response.Content.ReadFromJsonAsync<List<Word>>();
            if (words is null)
            {
                return new ViResult<List<Word>>(ViResultTypes.Fail, null, methodName, $"Words of dict {dictGuid} from api is null ref.");
            }

            if (words is not null && words.Count > 0)
            {
                return new ViResult<List<Word>>(ViResultTypes.Founded, words, methodName, $"Words for dict {dictGuid} list capacity is {words.Count}.");
            }
        }
        else
        {
            ApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
            if (apiResponse is not null && apiResponse.Message is not null)
            {
                return new ViResult<List<Word>>(ViResultTypes.Fail, null, methodName, $"{apiResponse.Message}.");
            }
        }
        return new ViResult<List<Word>>(ViResultTypes.Fail, null, methodName, $"Bad response from api, status code: {response.StatusCode}.");
    }
    public async Task<ViResult<bool>> AddNewWordToApiAsync(string jwt, Guid dictGuid, string sourceWord, string targetWord)
    {
        string methodName = nameof(AddNewWordToApiAsync);
        string url = $"{hostname}/api/words/add";

        if (dictGuid == Guid.Empty || string.IsNullOrWhiteSpace(sourceWord) || string.IsNullOrWhiteSpace(targetWord))
        {
            return new ViResult<bool>(ViResultTypes.Fail, false, methodName, $"Bad input.");
        }

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        var word = new { dictguid = dictGuid, sourceword = sourceWord, targetword = targetWord };
        JsonContent content = JsonContent.Create(word);
        using var response = await httpClient.PostAsync(url, content);
        if (response.IsSuccessStatusCode)
        {
            return new ViResult<bool>(ViResultTypes.Created, true, methodName, $"Word {sourceWord}{targetWord} added to dict {dictGuid} in api.");
        }

        return new ViResult<bool>(ViResultTypes.Fail, false, methodName, $"Word {sourceWord}:{targetWord} not added to dict {dictGuid} in api.");
    }
    public async Task<ViResult<Word>> EditRatingWordInApi(string jwt, Guid wordGuid, RatingAction action)
    {
        string methodName = nameof(EditRatingWordInApi);

        string url = string.Empty;
        if (action is RatingAction.Increase) { url = $"{hostname}/api/words/increase/{wordGuid}"; }
        if (action is RatingAction.Decrease) { url = $"{hostname}/api/words/decrease/{wordGuid}"; }

        if (wordGuid == Guid.Empty)
        {
            return new ViResult<Word>(ViResultTypes.Fail, null, methodName, $"Bad input.");
        }

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        using var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            Word? word = await response.Content.ReadFromJsonAsync<Word>();
            if (word is null)
            {
                return new ViResult<Word>(ViResultTypes.Fail, null, methodName, $"Bad deseralize.");
            }

            if (word is not null)
            {
                return new ViResult<Word>(ViResultTypes.Updated, word, methodName, $"Word {wordGuid} rating is {word.Rating}.");
            }
        }
        else
        {
            ApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
            if (apiResponse?.Message is not null)
            { 
                return new ViResult<Word>(ViResultTypes.Fail, null, methodName, apiResponse.Message); 
            }
        }
        return new ViResult<Word>(ViResultTypes.Fail, null, methodName, $"Word {wordGuid} rating not changed in api.");
    }
    public async Task<ViResult<bool>> RemoveWordFromApiAsync(string jwt, Guid wordGuid)
    {
        string methodName = nameof(RemoveWordFromApiAsync);
        string url = $"{hostname}/api/words/remove/{wordGuid}";

        if (wordGuid == Guid.Empty)
        {
            return new ViResult<bool>(ViResultTypes.Fail, false, methodName, $"Bad input.");
        }

        httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);
        using var response = await httpClient.GetAsync(url);
        if (response.IsSuccessStatusCode)
        {
            return new ViResult<bool>(ViResultTypes.Removed, true, methodName, $"Word {wordGuid} removed.");
        }
        else
        {
            ApiResponse? apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse>();
            if (apiResponse is not null && apiResponse.Message is not null)
            {
                return new ViResult<bool>(ViResultTypes.Fail, false, methodName, apiResponse.Message);
            }
        }
        return new ViResult<bool>(ViResultTypes.Fail, false, methodName, $"Not removed from api. Status code: {response.StatusCode}.");
    }
}
public enum RatingAction { Increase, Decrease}