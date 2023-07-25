using System.Reflection;
using ViTelegramBot.Entities;
using ViTelegramBot.Http.JsonEntites;

namespace ViTelegramBot.ApiRequesting;

public partial class ViApiClient
{
    public async Task<ViResult<List<Word>>> GetWordsAsync(long id, Guid dictGuid)
    {
        string methodName = nameof(GetWordsAsync);

        ViResult<string> getJwtResult = await GetJwtAsync(id);
        if (getJwtResult.ResultCode is ViResultTypes.Fail || getJwtResult.ResultValue is null)
        {
            return new ViResult<List<Word>>(ViResultTypes.Fail, null, methodName, $"Jwt for chatId: {id} not found.");
        }

        if (getJwtResult.ResultCode is ViResultTypes.Founded && getJwtResult.ResultValue is not null)
        {
            string jwt = getJwtResult.ResultValue;
            ViResult<List<Word>> getWordResult = await ApiHttpClient.GetWordsFromApiAsync(jwt, dictGuid);
            if (getWordResult.ResultCode is ViResultTypes.Founded && getWordResult.ResultValue is not null)
            {
                return new ViResult<List<Word>>(ViResultTypes.Founded, getWordResult.ResultValue, methodName, $"Recieved from api words from dict {dictGuid}. Capacity: {getWordResult.ResultValue.Count}");
            }
        }
        return new ViResult<List<Word>>(ViResultTypes.Fail, null, methodName, $"Bad response from API, maybe word not exists.");
    }
    public async Task<ViResult<bool>> AddNewWord(long id, Guid dictGuid, string sourceWord, string targetWord)
    {
        string methodName = nameof(AddNewWord);

        ViResult<string> getJwtResult = await GetJwtAsync(id);
        if (getJwtResult.ResultCode is ViResultTypes.Fail || getJwtResult.ResultValue is null)
        {
            return new ViResult<bool>(ViResultTypes.Fail, false, methodName, $"Jwt for chatId: {id} not found.");
        }

        if (getJwtResult.ResultCode is ViResultTypes.Founded && getJwtResult.ResultValue is not null)
        {
            string jwt = getJwtResult.ResultValue;
            ViResult<bool> addWordResult = await ApiHttpClient.AddNewWordToApiAsync(jwt, dictGuid, sourceWord, targetWord);
            if (addWordResult.ResultCode is ViResultTypes.Created && addWordResult.ResultValue is not false)
            {
                return new ViResult<bool>(ViResultTypes.Founded, addWordResult.ResultValue, methodName, addWordResult.Message);
            }
            else
            {
                return new ViResult<bool>(ViResultTypes.Fail, false, methodName, addWordResult.Message);
            } 
                
        }
        return new ViResult<bool>(ViResultTypes.Fail, false, methodName, $"Bad response from API, maybe no dict or affilation.");
    }
    public async Task<ViResult<Word>> EditRatingWord(long id, Guid wordGuid, RatingAction action)
    {
        string methodName = nameof(EditRatingWord);

        ViResult<string> getJwtResult = await GetJwtAsync(id);
        if (getJwtResult.ResultCode is ViResultTypes.Fail || getJwtResult.ResultValue is null)
        {
            return new ViResult<Word>(ViResultTypes.Fail, null, methodName, $"Jwt for chatId: {id} not found.");
        }
        if (getJwtResult.ResultCode is ViResultTypes.Founded && getJwtResult.ResultValue is not null)
        {
            string jwt = getJwtResult.ResultValue;
            ViResult<Word> editRatingResult = await ApiHttpClient.EditRatingWordInApi(jwt, wordGuid, action);
            if (editRatingResult.ResultCode is ViResultTypes.Updated && editRatingResult.ResultValue is not null)
            {
                return new ViResult<Word>(ViResultTypes.Updated, editRatingResult.ResultValue, methodName, editRatingResult.Message);
            }
            else
            {
                return new ViResult<Word>(ViResultTypes.Fail, null, methodName, editRatingResult.Message);
            }
        }
        return new ViResult<Word>(ViResultTypes.Fail, null, methodName, $"Bad response from API, maybe no word or affilation.");
    }
}
