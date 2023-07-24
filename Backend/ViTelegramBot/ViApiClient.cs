using Newtonsoft.Json.Linq;
using System.Reflection;
using Telegram.Bot.Types;
using ViTelegramBot.Entities;
using ViTelegramBot.Http;
using ViTelegramBot.Http.JsonEntites;

namespace ViTelegramBot;

public class ViApiClient
{
    private ViSessionList SessionList { get; }
    private ViApiHttpClient ApiHttpClient { get; }
    public ViApiClient(string hostname, string sessionListPath)
    {
        SessionList = new ViSessionList(sessionListPath);
        ApiHttpClient = new ViApiHttpClient(hostname);
    }


    private async Task<ViResult<string>> GetJwtAsync(long id)
    {
        string methodName = nameof(GetJwtAsync);

        ViSession? session = SessionList.Where(s => s.TelegramId == id).FirstOrDefault();

        if (session is not null && (session.IsNotExpried() is true)) //Если Jwt есть в сессиях и действителен
        {
            return new ViResult<string>(ViResultTypes.Founded, session.JwtToken, methodName, $"Jwt for chatId: {id} was found in local storage.");
        }


        if (session is not null && (session.IsNotExpried() is false)) //Если Jwt есть в сессиях, но устарел
        {
            ViResult<string> apiResult = await ApiHttpClient.GetJwtFromApiAsync(id);
            if (apiResult.ResultCode is ViResultTypes.Founded && apiResult.ResultValue is not null)
            {
                string jwtToken = apiResult.ResultValue;
                SessionList.Remove(session);
                SessionList.Add(new ViSession(id, jwtToken));
                return new ViResult<string>(ViResultTypes.Founded, jwtToken, methodName, $"Jwt for chatId: {id} was found in API database and updated locally.");
            }
        }

        {
            ViResult<string> apiResult = await ApiHttpClient.GetJwtFromApiAsync(id);
            if (apiResult.ResultCode is ViResultTypes.Founded && apiResult.ResultValue is not null)
            {
                string jwtToken = apiResult.ResultValue;
                SessionList.Add(new ViSession(id, jwtToken));
                return new ViResult<string>(ViResultTypes.Founded, jwtToken, methodName, $"Jwt for chatId: {id} was found in API database and added locally.");
            }
        }

        return new ViResult<string>(ViResultTypes.NotFounded, null!, methodName, $"Jwt for chatId: {id} was not found.");
    }

    public async Task<ViResult<string>> SignUpUserAsync(long id, string firstname)
    {
        string methodName = nameof(SignUpUserAsync);

        ViResult<string> tryGetJwtResult = await GetJwtAsync(id);
        if (tryGetJwtResult.ResultCode is ViResultTypes.Founded || tryGetJwtResult.ResultValue is not null)
        {
            return new ViResult<string>(ViResultTypes.Founded, tryGetJwtResult.ResultValue, methodName, $"Jwt for chatId: {id} already exists.");
        }

        ViResult<string> addApiResult = await ApiHttpClient.AddUserToApi(id, firstname);
        if (addApiResult.ResultCode is ViResultTypes.Created && addApiResult.ResultValue is not null)
        {
            addApiResult.MethodName = methodName;
            return addApiResult;
        }

        return new ViResult<string>(ViResultTypes.Fail, null, methodName, $"The user chatid:{id} was not able to add.");
    }
    
    public async Task<ViResult<List<ViDictionary>>> GetDictList(long id)
    {
        string methodName = nameof(GetDictList);

        ViResult<string> getJwtResult = await GetJwtAsync(id);
        if (getJwtResult.ResultCode is ViResultTypes.NotFounded || getJwtResult.ResultValue is null)
        {
            return new ViResult<List<ViDictionary>>(ViResultTypes.NotFounded, null, methodName, $"Jwt for chatId: {id} not found.");
        }

        if (getJwtResult.ResultCode is ViResultTypes.Founded && getJwtResult.ResultValue is not null)
        {
            string jwt = getJwtResult.ResultValue;
            ViResult<List<ViDictionary>> getDictsResult = await ApiHttpClient.GetDictsFromApiAsync(jwt);
            if (getDictsResult.ResultCode is ViResultTypes.Founded && getDictsResult.ResultValue is not null)
            {
                return new ViResult<List<ViDictionary>>(ViResultTypes.Founded, getDictsResult.ResultValue, methodName, $"Dict found, Capacity: {getDictsResult.ResultValue.Count}.");
            }
        }

        return new ViResult<List<ViDictionary>>(ViResultTypes.NotFounded, null, methodName, $"Bad response from API.");
    }

    public async Task<ViResult<Guid>> AddDictionary(long id, string dictName)
    {
        string methodName = nameof(AddDictionary);

        ViResult<string> getJwtResult = await GetJwtAsync(id);
        if (getJwtResult.ResultCode is ViResultTypes.NotFounded || getJwtResult.ResultValue is null)
        {
            return new ViResult<Guid>(ViResultTypes.NotFounded, Guid.Empty, methodName, $"Jwt for chatId: {id} not found.");
        }

        if (getJwtResult.ResultCode is ViResultTypes.Founded && getJwtResult.ResultValue is not null)
        {
            string jwt = getJwtResult.ResultValue;
            ViResult<Guid> addDictResult = await ApiHttpClient.AddNewDictToApiAsync(jwt, dictName);
            if (addDictResult.ResultCode is ViResultTypes.Created && addDictResult.ResultValue != Guid.Empty)
            {
                return new ViResult<Guid>(ViResultTypes.Created, addDictResult.ResultValue, methodName, $"Dict with name {dictName} created. Guid is {addDictResult.ResultValue}.");
            }
            else
            {
                return new ViResult<Guid>(ViResultTypes.Fail, Guid.Empty, methodName, addDictResult.Message);
            }
        }
        return new ViResult<Guid>(ViResultTypes.Fail, Guid.Empty, methodName, "Ne poluchilos.");
    }
}
