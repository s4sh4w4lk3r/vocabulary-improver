using Microsoft.AspNetCore.Cors.Infrastructure;
using ViAPI.Auth;
using ViAPI.Database;
using ViAPI.Entites.DTO;
using ViAPI.Entities;
using ViAPI.Entities.JsonModels;

namespace ViAPI.Other;

public static class EndpointMethods
{
    private const int JWT_DURATION_MINUTES = 1440; //(24 hours).
    public static IResult GetDictsByUserFromContext(HttpContext http, ViDbContext db)
    {
        ViResult<Guid> userGuidResult = Accounting.TryGetGuidFromHttpContext(http);
        if (userGuidResult.ResultCode is not ViResultTypes.GetGuidFromHttpOk)
        {
            return Results.BadRequest(userGuidResult);
        }
        Guid userGuid = userGuidResult.ResultValue;

        ViResult<List<ViDictionaryDto>> dtoDictsResult = db.GetDictionariesDtoByUser(userGuid);
        if (dtoDictsResult.ResultCode is ViResultTypes.Founded && dtoDictsResult.ResultValue is not null)
        {
            return Results.Ok(dtoDictsResult.ResultValue);
        }
        else
        {
            return Results.BadRequest(dtoDictsResult);
        }
    }
    public static IResult GetWords(HttpContext http, ViDbContext db, Guid dictGuid)
    {
        ViResult<Guid> userGuidResult = Accounting.TryGetGuidFromHttpContext(http);
        if (userGuidResult.ResultCode is not ViResultTypes.GetGuidFromHttpOk)
        {
            return Results.BadRequest(userGuidResult.ResultCode.ToString());
        }
        Guid userGuid = userGuidResult.ResultValue;

        bool dictGuidOk = dictGuid.IsNotEmpty();
        if (dictGuidOk is false)
        {
            return Results.BadRequest("Dict guid is empty.");
        }

        ViResult<List<WordDto>> dtoWordsResult = db.GetWordsDtoByDict(userGuid, dictGuid);
        if (dtoWordsResult.ResultCode is ViResultTypes.Founded && dtoWordsResult.ResultValue is not null)
        {
            return Results.Ok(dtoWordsResult.ResultValue);
        }
        else
        {
            return Results.BadRequest(dtoWordsResult);
        }
    }
    public static IResult EditRating(HttpContext http, ViDbContext db, Guid wordGuid, RatingAction action)
    {
        ViResult<Guid> userGuidResult = Accounting.TryGetGuidFromHttpContext(http);
        if (userGuidResult.ResultCode is not ViResultTypes.GetGuidFromHttpOk)
        {
            return Results.BadRequest(userGuidResult);
        }
        Guid userGuid = userGuidResult.ResultValue;

        bool wordGuidOk = wordGuid.IsNotEmpty();
        if (wordGuidOk is false)
        {
            return Results.BadRequest("Word guid is empty.");
        }

        ViResult<Word> wordResults = db.UpdateWordRatingDb(userGuid, wordGuid, action);
        if (wordResults.ResultCode is ViResultTypes.Updated && wordResults.ResultValue is not null)
        {
            Word word = wordResults.ResultValue;
            return Results.Ok(new { guid = word.Guid, sourceword = word.SourceWord, targetword = word.TargetWord, rating = word.Rating });
        }
        else
        {
            return Results.BadRequest(wordResults);
        }

    }
    public static IResult GetJwtByTelegramId(ViDbContext db, string idString)
    {
        bool tgIdOk = ulong.TryParse(idString, out ulong id);
        if (tgIdOk is false)
        {
            return Results.BadRequest($"Can't parse TelegramId: {idString}.");
        }

        ViResult<Guid> userGuidResult = db.TryGetGuidFromTgId(id);
        Guid guid = userGuidResult.ResultValue;
        if (userGuidResult.ResultCode is ViResultTypes.NotFoundDb)
        {
            return Results.BadRequest(userGuidResult);
        }

        string jwt = Accounting.GenerateJwt(guid, JWT_DURATION_MINUTES);
        return Results.Ok(new { jwttoken = jwt });
    }
    public async static Task<IResult> GetJwtByLoginAsync(HttpContext http, ViDbContext db)
    {
        if (http.Request.HasJsonContentType() is false)
        {
            return Results.BadRequest("Bad ContentType.");
        }

        try
        {
            RegistredUserJson? user = await http.Request.ReadFromJsonAsync<RegistredUserJson>();
            if (user is null)
            {
                return Results.BadRequest("User instance from JSON is null.");
            }

            string username = user.Username!;
            string password = user.Password!;
            if (InputChecker.CheckString(username, password) is false)
            {
                return Results.BadRequest("Bad format username or password.");
            }

            bool userIdentifed = db.IdentifyUser(username, password, out Guid guid);
            if (userIdentifed is true)
            {
                string jwt = Accounting.GenerateJwt(guid, JWT_DURATION_MINUTES);
                return Results.Ok(new { jwttoken = jwt });
            }
            else
            {
                return Results.BadRequest("Invalid username or password.");
            };
        }
        catch
        {
            return Results.BadRequest("Bad Json Deserializing.");
        }
    }
    public static IResult AddDictionary(HttpContext http, ViDbContext db, string name)
    {
        ViResult<Guid> userGuidResult = Accounting.TryGetGuidFromHttpContext(http);
        if (userGuidResult.ResultCode is not ViResultTypes.GetGuidFromHttpOk)
        {
            return Results.BadRequest(userGuidResult.ResultCode.ToString());
        }
        Guid userGuid = userGuidResult.ResultValue;

        bool nameOk = InputChecker.CheckString(name) && name.Length < 255;
        if (nameOk is false)
        {
            return Results.BadRequest($"The string: {name} has bad format, maybe length > 254 chars.");
        }

        ViResult<ViDictionary> dictResult = db.AddDictionary(name, userGuid);
        if (dictResult.ResultValue is not null && dictResult.ResultCode is ViResultTypes.Created)
        {
            return Results.Ok( new {dictGuid = dictResult.ResultValue.Guid });
        }
        else
        {
            return Results.BadRequest(dictResult);
        }
    }
    public static IResult RemoveDictionary(HttpContext http, ViDbContext db, Guid dictGuid)
    {
        ViResult<Guid> userGuidResult = Accounting.TryGetGuidFromHttpContext(http);
        if (userGuidResult.ResultCode is not ViResultTypes.GetGuidFromHttpOk)
        {
            return Results.BadRequest(userGuidResult.ResultCode.ToString());
        }
        Guid userGuid = userGuidResult.ResultValue;

        bool dictGuidOk = dictGuid.IsNotEmpty();
        if (dictGuidOk is false)
        {
            return Results.BadRequest("Dict guid is empty.");
        }

        ViResult<ViDictionary> removedResult = db.RemoveDictionary(userGuid, dictGuid);
        if (removedResult.ResultCode is ViResultTypes.Removed && removedResult.ResultValue is not null)
        {
            return Results.Ok(new {dictGuid = removedResult.ResultValue.Guid });
        }
        else
        {
            return Results.BadRequest(removedResult);
        }
    }
    public static IResult EditDictionaryName(HttpContext http, ViDbContext db, Guid dictGuid, string name)
    {
        ViResult<Guid> userGuidResult = Accounting.TryGetGuidFromHttpContext(http);
        if (userGuidResult.ResultCode is not ViResultTypes.GetGuidFromHttpOk)
        {
            return Results.BadRequest(userGuidResult.ResultCode.ToString());
        }
        Guid userGuid = userGuidResult.ResultValue;

        bool wordGuidOk = dictGuid.IsNotEmpty();
        if (wordGuidOk is false)
        {
            return Results.BadRequest("Word guid is empty.");
        }

        bool nameOk = InputChecker.CheckString(name) && name.Length < 255;
        if (nameOk is false)
        {
            return Results.BadRequest($"The string: {name} has bad format, maybe length > 254 chars.");
        }


        ViResult<ViDictionary> dictResult = db.UpdateDictionaryNameDb(userGuid, dictGuid, name);
        if (dictResult.ResultCode is ViResultTypes.Updated && dictResult.ResultValue is not null)
        {
            return Results.Ok(new {dictGuid, name = dictResult.ResultValue.Name });
        }
        else
        {
            return Results.BadRequest(dictResult);
        }
    }
    public async static Task<IResult> AddWordAsync(HttpContext http, ViDbContext db)
    {
        if (http.Request.HasJsonContentType() is false)
        {
            return Results.BadRequest("Bad content-type.");
        }

        ViResult<Guid> userGuidResult = Accounting.TryGetGuidFromHttpContext(http);
        if (userGuidResult.ResultCode is not ViResultTypes.GetGuidFromHttpOk)
        {
            return Results.BadRequest(userGuidResult.ResultCode.ToString());
        }
        Guid userGuid = userGuidResult.ResultValue;

        try
        {
            WordJson? wordJson = await http.Request.ReadFromJsonAsync<WordJson>();
            if (wordJson is null)
            {
                return Results.BadRequest("Word instance from JSON is null.");
            }

            if (wordJson.DictGuid.IsNotEmpty() is false)
            {
                return Results.BadRequest("Empty dict guid from Json.");
            }

            if (InputChecker.CheckString(wordJson.TargetWord, wordJson.SourceWord) is false)
            {
                return Results.BadRequest("Source or target word is bad format.");
            }

            if (wordJson?.TargetWord?.Length > 254 || wordJson?.SourceWord?.Length > 254 is true)
            {
                return Results.BadRequest("Source or target word is so long.");
            }

            Guid dictGuid = wordJson!.DictGuid;
            string sourceWord = wordJson.SourceWord!;
            string targetWord = wordJson.TargetWord!;

            ViResult<Word> wordResult = db.AddWord(userGuid, sourceWord, targetWord, dictGuid);
            if (wordResult.ResultValue is not null && wordResult.ResultCode is ViResultTypes.Created)
            {
                return Results.Ok(new { wordGuid = wordResult.ResultValue.Guid, dictGuid = wordResult.ResultValue.DictionaryGuid});
            }
            else
            {
                return Results.BadRequest(wordResult);
            }
        }
        catch (Exception)
        {
            return Results.BadRequest("Bad Json.");
        }
    }
    public static IResult RemoveWord(HttpContext http, ViDbContext db, Guid wordGuid)
    {
        ViResult<Guid> userGuidResult = Accounting.TryGetGuidFromHttpContext(http);
        if (userGuidResult.ResultCode is not ViResultTypes.GetGuidFromHttpOk)
        {
            return Results.BadRequest(userGuidResult.ResultCode.ToString());
        }
        Guid userGuid = userGuidResult.ResultValue;

        bool wordGuidOk = wordGuid.IsNotEmpty();
        if (wordGuidOk is false)
        {
            return Results.BadRequest("Word guid is empty.");
        }

        ViResult<Word> removeResult = db.RemoveWord(userGuid, wordGuid);
        if (removeResult.ResultCode is ViResultTypes.Removed && removeResult.ResultValue is not null)
        { 
            return Results.Ok( new {wordGuid = removeResult.ResultValue.Guid }); 
        }
        else
        {
            return Results.BadRequest(ViResultTypes.NotFoundOrNoAffilationDb.ToString());
        }
    }
    public async static Task<IResult> RegisterTelegramUserAsync(HttpContext http, ViDbContext db)
    {
        if (http.Request.HasJsonContentType() is false) 
        { 
            return Results.BadRequest("Bad content-type."); 
        }

        try
        {
            TelegramUserJson? userJson = await http.Request.ReadFromJsonAsync<TelegramUserJson>();
            bool idOk = ulong.TryParse(userJson?.TelegramId, out ulong id);
            if (idOk is false)
            {
                return Results.BadRequest("TelgramId bad parse.");
            }

            bool firstnameOk = InputChecker.CheckString(userJson?.Firstname) && userJson?.Firstname?.Length < 255;
            if (firstnameOk is false)
            {
                return Results.BadRequest("Firstname bad format or length > 254.");
            }

            ViResult<TelegramUser> addUserResult = db.AddTelegramUser(id, userJson?.Firstname!);

            if (addUserResult.ResultCode == ViResultTypes.Created && (addUserResult.ResultValue is not null) is true)
            {
                return Results.Ok(new {userGuid = addUserResult.ResultValue.Guid });
            }
            else
            {
                return Results.BadRequest(addUserResult);
            }
        }
        catch (Exception)
        {
            return Results.BadRequest("Bad Json.");
        }
    }
    public async static Task<IResult> RegisterRegistredUserAsync(HttpContext http, ViDbContext db)
    {
        if (http.Request.HasJsonContentType() is false) 
        { 
            return Results.BadRequest("Bad content-type."); 
        }

        try
        {
            RegistredUserJson? userJson = await http.Request.ReadFromJsonAsync<RegistredUserJson>();

            bool emailOk = userJson is not null ? userJson.Email.IsEmail() : false;
            if (emailOk is false)
            {
                return Results.BadRequest("Bad email format.");
            }

            bool stringsOk = InputChecker.CheckString(userJson?.Username, userJson?.Password, userJson?.Firstname);
            if (stringsOk is false)
            {
                return Results.BadRequest("Username || password || firstname is null, empty, or contain whitespace.");
            }

            string username = userJson!.Username!;
            string email = userJson!.Email!;
            string firstname = userJson!.Firstname!;
            string hash = Accounting.GenerateHash(userJson!.Password!);

            ViResult<RegistredUser> addUserResult = db.AddRegistredUser(username, email, firstname, hash);
            if (addUserResult.ResultCode == ViResultTypes.Created && addUserResult.ResultValue is not null)
            {
                return Results.Ok(new { userGuid = addUserResult.ResultValue.Guid });
            }
            else
            {
                return Results.BadRequest(addUserResult);
            }
        }
        catch (Exception)
        {
            return Results.BadRequest("Bad Json.");
        }
    }
}