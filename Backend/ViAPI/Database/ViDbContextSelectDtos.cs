using ViAPI.Entites.DTO;
using ViAPI.Other;

namespace ViAPI.Database;

public partial class ViDbContext
{
    public List<ViDictionaryDto>? GetDictionariesDtoByUser(Guid userGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        var dictsDto = Dictionaries.Where(p => p.UserGuid == userGuid).Select(p => new ViDictionaryDto(p.Guid, p.Name)).ToList();

        if (dictsDto is not null && dictsDto?.Count > 0)
        {
            Logger?.LogInformation($"Method {methodName}, Status: OK. User {userGuid} found, Count: {dictsDto.Count}");
            return dictsDto;
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: Fail. User {userGuid} not found.");
            return null;
        }
    }
    public List<WordDto>? GetWordsDtoByDict(Guid userGuid, Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        bool userGuidOk = userGuid.IsNotEmpty();
        bool dictGuidOk = dictGuid.IsNotEmpty();

        if (dictGuidOk && userGuidOk is false) 
        {
            Logger?.LogWarning($"Method {methodName}, Status: Fail. The combination of User {userGuid} and dict {dictGuid} not found.");
            return null;
        }

        List<WordDto>? wordsDto = Words.Where(p => p.DictionaryGuid == dictGuid && p.Dictionary!.UserGuid == userGuid).Select(p => new WordDto(p.Guid, p.SourceWord, p.TargetWord, p.Rating)).ToList();

        if (wordsDto is not null && wordsDto.Count > 0)
        {
            Logger?.LogInformation($"Method {methodName}, Status: OK. Dict {dictGuid} found, Count: {wordsDto.Count}");
            return wordsDto;
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: Fail. The combination of User {userGuid} and dict {dictGuid} not found.");
            return null;
        }
    }
}
