using ViAPI.Auth;
using ViAPI.Database;
using ViAPI.Entites.DTO;
using ViAPI.Entities;

namespace ViAPI.StaticMethods;

public static class EndpointMethods
{
    public static IResult GetDictsByUserFromContext(HttpContext context, ViDbContext db)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(context, out Guid userGuid);

        if (userGuidOk is true)
        {
            List<ViDictionaryDto>? dtoDicts = db.GetDictionariesDtoByUser(userGuid);
            return (dtoDicts is not null) ? Results.Ok(dtoDicts) : Results.BadRequest($"User`s {userGuid} dicts not found.");
        }
        else
        {
            return Results.Unauthorized();
        }
    }
    public static IResult GetWords(HttpContext context, ViDbContext db, Guid dictGuid)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(context, out Guid userGuid);
        bool dictGuidOk = dictGuid.IsNotEmpty();

        if (dictGuidOk && userGuidOk is true)
        {
            List<WordDto>? dtoWords = db.GetWordsDtoByDict(userGuid, dictGuid);
            return dtoWords is not null ? Results.Ok(dtoWords) : Results.BadRequest($"User {userGuid} or dict {dictGuid} not found.");
        }
        else
        {
            return Results.Unauthorized();
        }
    }
    public static IResult EditRating(HttpContext context, ViDbContext db, Guid wordGuid, ViDbContext.RatingAction action)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(context, out Guid userGuid);
        bool wordGuidOk = wordGuid.IsNotEmpty();

        if (userGuidOk && wordGuidOk is true)
        {
            Word? word = db.UpdateRatingDbAsync(wordGuid, action).Result;
            return (word is not null) ? Results.Ok(new { guid = word.Guid, sourceword = word.SourceWord, targetword = word.TargetWord, rating = word.Rating })
                : Results.BadRequest($"Word {wordGuid} not found.");
        }
        else
        {
            return Results.Unauthorized();
        }
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
        return Results.BadRequest($"User not found with TgId {idString}");
    }
}