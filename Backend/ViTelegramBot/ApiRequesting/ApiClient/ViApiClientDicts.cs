﻿using ViTelegramBot.Entities;
using ViTelegramBot.Http.JsonEntites;

namespace ViTelegramBot.ApiRequesting;

public partial class ViApiClient
{
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
