using System.Collections.Generic;
using ViAPI.Auth;
using ViAPI.Database;
using ViAPI.Entites.DTO;
using ViAPI.Entities;

namespace ViAPI.StaticMethods;

public static class EndpointMethods
{
    static ILogger Logger { get; set; }
    static EndpointMethods()
    {
        Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(typeof(EndpointMethods).Name);
    }
    public static List<ViDictionaryDto> GetDicts(HttpContext context, ViDbContext db)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (Accounting.IsContextHasGuid(context, out Guid userGuid) is true)
        {
            Logger?.LogInformation($"Method {methodName}, Status: OK. User {userGuid} has been identified, searching dicts...");
            var dtoDicts = db.GetDictionariesDtoByUser(userGuid)!;
            return dtoDicts;
        }
        else
        {
            return new List<ViDictionaryDto>();
        }
    }
    public static List<WordDto> GetWords(ViDbContext db, Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (dictGuid.IsNotEmpty() is true)
        {
            Logger?.LogInformation($"Method {methodName}, Status: OK. Dict {dictGuid} recieved, searching words...");
            var dtoWords = db.GetWordsDtoByDict(dictGuid)!;
            return dtoWords;
        }
        else
        {
            return new List<WordDto>();
        }
    }
    public static IResult EditRating(ViDbContext db, Guid wordGuid, ViDbContext.RatingAction action)
    {
        int rating = db.EditRatingDbAsync(wordGuid, action).Result;

        if (rating >= 0)
        {
#warning    ебануть логгера сюда ко всем методам
            return Results.Ok(new {wordGuid, rating});
        }
        else
        {
            return Results.BadRequest($"Word {wordGuid} not found.");
        }
    }
}
