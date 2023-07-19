using ViAPI.Auth;
using ViAPI.Database;
using ViAPI.Entites.DTO;
using ViAPI.Entities;
using ViAPI.Entities.DTO;

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
            UserDto? user = await request.ReadFromJsonAsync<UserDto>();

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
}