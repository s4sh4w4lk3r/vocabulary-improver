using ViAPI.Auth;
using ViAPI.Database;
using ViAPI.Entites.DTO;
using ViAPI.Entities;
using ViAPI.Entities.JsonModels;

namespace ViAPI.Other;

public static class EndpointMethods
{
    public static IResult GetDictsByUserFromContext(HttpContext http, ViDbContext db)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        if (userGuidOk is false)
        {
            return Results.BadRequest("User guid not recognized.");
        }

        List<ViDictionaryDto>? dtoDicts = db.GetDictionariesDtoByUser(userGuid);
        if (dtoDicts is not null)
        {
            return Results.Ok(dtoDicts);
        }
        else
        {
            return Results.BadRequest($"Bad response from database.");
        }
    }
    public static IResult GetWords(HttpContext http, ViDbContext db, Guid dictGuid)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        if (userGuidOk is false)
        {
            return Results.BadRequest("User guid not recognized.");
        }

        bool dictGuidOk = dictGuid.IsNotEmpty();
        if (dictGuidOk is false)
        {
            return Results.BadRequest("Dict guid is empty.");
        }

        List<WordDto>? dtoWords = db.GetWordsDtoByDict(userGuid, dictGuid);
        if (dtoWords is not null)
        {
            return Results.Ok(dtoWords);
        }
        else
        {
            return Results.BadRequest($"Bad response from database.");
        }
    }
    public static IResult EditRating(HttpContext http, ViDbContext db, Guid wordGuid, ViDbContext.RatingAction action)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        if (userGuidOk is false)
        {
            return Results.BadRequest("User guid not recognized.");
        }

        bool wordGuidOk = wordGuid.IsNotEmpty();
        if (wordGuidOk is false)
        {
            return Results.BadRequest("Word guid is empty.");
        }

        Word? word = db.UpdateWordRatingDb(userGuid, wordGuid, action);
        if (word is not null)
        {
            return Results.Ok(new { guid = word.Guid, sourceword = word.SourceWord, targetword = word.TargetWord, rating = word.Rating });
        }
        else
        {
            return Results.BadRequest($"Bad response from database.");
        }

    }
    public static IResult GetJwtByTelegramId(ViDbContext db, string idString)
    {
        bool tgIdOk = ulong.TryParse(idString, out ulong id);
        if (tgIdOk is false)
        {
            Results.BadRequest($"Can't parse TelegramId: {idString}.");
        }

        bool userGuidOk = db.TryGetGuidFromTgId(id, out Guid guid);
        if (userGuidOk is false)
        {
            Results.BadRequest($"The user with the TelegramId: {id} was not found in the database.");
        }

        string jwt = Accounting.GenerateJwt(guid);
        return Results.Ok(new { jwt });
    }
    public async static Task<IResult> GetJwtByLogin(HttpContext http, ViDbContext db)
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
                string jwt = Accounting.GenerateJwt(guid, 20);
                return Results.Ok(new { jwt });
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
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        if (userGuidOk is false)
        {
            return Results.BadRequest("User guid not recognized.");
        }

        bool nameOk = InputChecker.CheckString(name) && name.Length < 255;
        if (nameOk is false)
        {
            return Results.BadRequest($"The string: {name} has bad format, maybe length > 254 chars.");
        }

        ViDictionary? dict = db.AddDictionary(name, userGuid);
        if (dict is not null)
        {
            return Results.Ok($"Dict {dict.Guid} added.");
        }
        else
        {
            return Results.BadRequest($"Bad response from database.");
        }
    }
    public static IResult RemoveDictionary(HttpContext http, ViDbContext db, Guid dictGuid)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        if (userGuidOk is false)
        {
            return Results.BadRequest("User guid not recognized.");
        }

        bool dictGuidOk = dictGuid.IsNotEmpty();
        if (dictGuidOk is false)
        {
            return Results.BadRequest("Dict guid is empty.");
        }

        bool removed = db.RemoveDictionary(userGuid, dictGuid);
        if (removed is true)
        {
            return Results.Ok($"Dict {dictGuid} removed.");
        }
        else
        {
            return Results.BadRequest($"Bad response from database.");
        }
    }
    public static IResult EditDictionaryName(HttpContext http, ViDbContext db, Guid dictGuid, string name)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        if (userGuidOk is false)
        {
            return Results.BadRequest("User guid not recognized.");
        }

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


        ViDictionary? dict = db.UpdateDictionaryNameDb(userGuid, dictGuid, name);
        if (dict is not null)
        {
            return Results.Ok($"New dict name is {name}");
        }
        else
        {
            return Results.BadRequest("Bad response from database.");
        }
    }
    public async static Task<IResult> AddWord(HttpContext http, ViDbContext db)
    {
        if (http.Request.HasJsonContentType() is false)
        {
            return Results.BadRequest("Bad content-type.");
        }

        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        if (userGuidOk is false)
        {
            return Results.BadRequest("User guid not recognized.");
        }

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

        if(wordJson?.TargetWord?.Length > 254 || wordJson?.SourceWord?.Length > 254 is true)
        {
            return Results.BadRequest("Source or target word is so long.");
        }

        Guid dictGuid = wordJson!.DictGuid;
        string sourceWord = wordJson.SourceWord!;
        string targetWord = wordJson.TargetWord!;

        Word? word = db.AddWord(userGuid, sourceWord, targetWord, dictGuid);
        if (word is not null)
        {
            return Results.Ok($"Word {word.Guid} added in dict {word.DictionaryGuid}.");
        }
        else
        {
            return Results.BadRequest("Bad response from database.");
        }

    }
    public static IResult RemoveWord(HttpContext http, ViDbContext db, Guid wordGuid)
    {
        bool userGuidOk = Accounting.TryGetGuidFromContext(http, out Guid userGuid);
        if (userGuidOk is false)
        {
            return Results.BadRequest("User guid not recognized.");
        }

        bool wordGuidOk = wordGuid.IsNotEmpty();
        if (wordGuidOk is false)
        {
            return Results.BadRequest("Word guid is empty.");
        }

        bool removed = db.RemoveWord(userGuid, wordGuid);
        if (removed is true)
        { 
            return Results.Ok($"Word {wordGuid} removed."); 
        }
        else
        {
            return Results.BadRequest("Bad response from database.");
        }
    }
    public async static Task<IResult> RegisterTelegramUser(HttpContext http, ViDbContext db)
    {
        if (http.Request.HasJsonContentType() is false) 
        { 
            return Results.BadRequest("Bad content-type."); 
        }

        TelegramUserJson? userJson = await http.Request.ReadFromJsonAsync<TelegramUserJson>();
        bool idOk = ulong.TryParse(userJson?.TelegramId, out ulong id);
        if (idOk is false)
        {
            return Results.BadRequest("TelgramId bad parse.");
        }


        bool firstnameOk = InputChecker.CheckString(userJson?.Firstname) && userJson?.Firstname?.Length < 255;
        if (firstnameOk is true)
        {
            var user = db.AddTelegramUser(id, userJson?.Firstname!);

            if (user is not null) 
            { 
                return Results.Ok($"User {user.Guid} added.");
            }
            else 
            {
                return Results.BadRequest("Bad response from database."); 
            }
        }
        return Results.BadRequest("Firstname bad format or length > 254.");
    }
    public async static Task<IResult> RegisterRegistredUser(HttpContext http, ViDbContext db)
    {
        if (http.Request.HasJsonContentType() is false) 
        { 
            return Results.BadRequest("Bad content-type."); 
        }

        RegistredUserJson? userJson = await http.Request.ReadFromJsonAsync<RegistredUserJson>();

        bool emailOk = userJson is not null ? userJson.Email.IsEmail() : false;
        if (emailOk is false) 
        {
            Results.BadRequest("Bad email format."); 
        }

        bool stringsOk = InputChecker.CheckString(userJson?.Username, userJson?.Password, userJson?.Firstname);
        if (stringsOk is false) 
        {
            Results.BadRequest("Username || password || firstname is null, empty, or contain whitespace."); 
        }

        string username = userJson!.Username!;
        string email = userJson!.Email!;
        string firstname = userJson!.Firstname!;
        string hash = Accounting.GenerateHash(userJson!.Password!);

        RegistredUser? user = db.AddRegistredUser(username, email, firstname, hash);
        if (user is not null) 
        {
            return Results.Ok($"User {user.Guid} added."); 
        } 
        else 
        { 
            return Results.BadRequest("User maybe already exists."); 
        }
    }
}