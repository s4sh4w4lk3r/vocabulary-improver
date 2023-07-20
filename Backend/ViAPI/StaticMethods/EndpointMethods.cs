using System.Net.WebSockets;
using ViAPI.Auth;
using ViAPI.Database;
using ViAPI.Entites.DTO;
using ViAPI.Entities;
using ViAPI.Entities.JsonModels;

namespace ViAPI.StaticMethods;

public static class EndpointMethods
{
    public static IResult GetDictsByUserFromContext(HttpContext http, ViDbContext db)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);

        if (userGuidOk is true)
        {
            List<ViDictionaryDto>? dtoDicts = db.GetDictionariesDtoByUser(userGuid);
            return (dtoDicts is not null) ? Results.Ok(dtoDicts) : Results.BadRequest($"User`s {userGuid} dicts not found.");
        }
        return Results.Unauthorized();
    }
    public static IResult GetWords(HttpContext http, ViDbContext db, Guid dictGuid)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        bool dictGuidOk = dictGuid.IsNotEmpty();

        if (dictGuidOk && userGuidOk is true)
        {
            List<WordDto>? dtoWords = db.GetWordsDtoByDict(userGuid, dictGuid);
            return dtoWords is not null ? Results.Ok(dtoWords) : Results.BadRequest($"The combination of User {userGuid} and dict {dictGuid} not found.");
        }
        return Results.Unauthorized();
    }
    public static IResult EditRating(HttpContext http, ViDbContext db, Guid wordGuid, ViDbContext.RatingAction action)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        bool wordGuidOk = wordGuid.IsNotEmpty();

        if (userGuidOk && wordGuidOk is true)
        {
            Word? word = db.UpdateWordRatingDb(userGuid, wordGuid, action);
            return (word is not null) ? Results.Ok(new { guid = word.Guid, sourceword = word.SourceWord, targetword = word.TargetWord, rating = word.Rating })
                : Results.BadRequest($"Word {wordGuid} not found.");
        }
        return Results.Unauthorized();
    }
    public static IResult GetJwtByTelegramId(ViDbContext db, string idString)
    {
        bool tgIdOk = ulong.TryParse(idString, out ulong id);
        bool userGuidOk = db.TryGetGuidFromTgId(id, out Guid guid);

        if (tgIdOk && userGuidOk is true)
        {
            string jwt = Accounting.GenerateJwt(guid);
            return Results.Ok(new { jwt });
        }
        return Results.BadRequest($"User not found with TelegramId: {idString}");
    }
    public async static Task<IResult> GetJwtByLogin(HttpContext http, ViDbContext db)
    {
        var request = http.Request;

        if (request.HasJsonContentType() is true)
        {
            RegistredUserJson? user = await request.ReadFromJsonAsync<RegistredUserJson>();

            if (user is not null)
            {
                string username = user.Username!;
                string password = user.Password!;

                if (InputChecker.CheckString(username, password))
                {
                    bool userIdentifed = db.IdentifyUser(username, password, out Guid guid);
                    return userIdentifed is true ? Results.Ok(Accounting.GenerateJwt(guid, 20)) : Results.BadRequest("Invalid username or password.");
                }
            }
        }
        return Results.BadRequest("Bad ContentType.");
    }
    public static IResult AddDictionary(HttpContext http, ViDbContext db, string name)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        bool nameOk = InputChecker.CheckString(name) && name.Length < 255;

        if (userGuidOk && nameOk is true)
        {
            var dict = db.AddDictionary(name, userGuid);
            return dict is not null ? Results.Ok($"Dict {dict.Guid} added.") : Results.BadRequest("User maybe not found.");
        }
        return Results.Unauthorized();
    }
    public static IResult RemoveDictionary(HttpContext http, ViDbContext db, Guid dictGuid)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        bool dictGuidOk = dictGuid.IsNotEmpty();
        if (userGuidOk && dictGuidOk is true)
        {
            bool removed = db.RemoveDictionary(userGuid, dictGuid);
            return removed is true ? Results.Ok($"Dict {dictGuid} removed.") : Results.BadRequest("Dict maybe not exists or not affiliated.");
        }
        return Results.Unauthorized();
    }
    public static IResult EditDictionaryName(HttpContext http, ViDbContext db, Guid dictGuid, string name)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        bool wordGuidOk = dictGuid.IsNotEmpty();
        bool nameOk = InputChecker.CheckString(name);

        if (userGuidOk && wordGuidOk && nameOk is true)
        {
            var dict = db.UpdateDictionaryNameDb(userGuid, dictGuid, name);
            return dict is not null ? Results.Ok($"New dict name is {name}") : Results.BadRequest("Dict maybe not exists or not affiliated.");
        }
        return Results.Unauthorized();
    }
    public async static Task<IResult> AddWord(HttpContext http, ViDbContext db)
    {
        if (http.Request.HasJsonContentType() is false) return Results.BadRequest("Bad content-type.");

        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);

        WordJson? wordJson = await http.Request.ReadFromJsonAsync<WordJson>();

        if (wordJson is not null && wordJson.DictGuid.IsNotEmpty() && InputChecker.CheckString(wordJson.TargetWord, wordJson.SourceWord) && userGuidOk is true)
        {
            Guid dictGuid = wordJson.DictGuid;
            string sourceWord = wordJson.SourceWord!;
            string targetWord = wordJson.TargetWord!;

            var word = db.AddWord(userGuid, sourceWord, targetWord, dictGuid);
            return word is not null ? Results.Ok("Word added.") : Results.BadRequest("Word not added.");
        }
        return Results.Unauthorized();
    }
    public static IResult RemoveWord(HttpContext http, ViDbContext db, Guid wordGuid)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        bool wordGuidOk = wordGuid.IsNotEmpty();
        if (userGuidOk && wordGuidOk is true)
        {
            bool removed = db.RemoveWord(userGuid, wordGuid);
            return removed is true ? Results.Ok($"Word {wordGuid} removed.") : Results.BadRequest("Word maybe not exists or not affiliated.");
        }
        return Results.Unauthorized();
    }
    public async static Task<IResult> RegisterTelegramUser(HttpContext http, ViDbContext db)
    {
        if (http.Request.HasJsonContentType() is false) { Results.BadRequest("Bad content-type."); }

        TelegramUserJson? userJson = await http.Request.ReadFromJsonAsync<TelegramUserJson>();

        bool idOk = ulong.TryParse(userJson?.TelegramId, out ulong id);
        bool firstnameOk = InputChecker.CheckString(userJson?.Firstname) && userJson?.Firstname?.Length < 255;

        if (firstnameOk && idOk is true)
        {
            var user = db.AddTelegramUser(id, userJson?.Firstname!);

            return user is not null ? Results.Ok($"User {user.Guid} added.") : Results.BadRequest("User maybe already exists.");
        }
        return Results.BadRequest("Bad input.");
    }
}