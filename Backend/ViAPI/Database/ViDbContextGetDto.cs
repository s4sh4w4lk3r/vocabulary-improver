using System.Net.WebSockets;
using ViAPI.Entites.DTO;
using ViAPI.Entities;

namespace ViAPI.Database;

public partial class ViDbContext
{
    public List<ViDictionaryDto> GetDictionariesDtoByUser(Guid userGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        var dictsDto = Dictionaries.Where(p => p.UserGuid == userGuid).Select(p => new ViDictionaryDto(p.Guid, p.Name)).ToList();

        if (dictsDto is not null)
        {
            Logger?.LogInformation($"Method {methodName}, Status: OK. User {userGuid} found, Count: {dictsDto.Count}");
            return dictsDto;
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: Fail. User {userGuid} not found.");
            return new List<ViDictionaryDto>();
        }
    }
    public List<WordDto> GetWordsDtoByDict(Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        var wordsDto = Words.Where(p => p.DictionaryGuid == dictGuid).Select(p => new WordDto(p.Guid, p.SourceWord, p.TargetWord, p.Rating)).ToList();

        if (wordsDto is not null)
        {
            Logger?.LogInformation($"Method {methodName}, Status: OK. Dict {dictGuid} found, Count: {wordsDto.Count}");
            return wordsDto;
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: Fail. Dict {dictGuid} not found.");
            return new List<WordDto>();
        }
    }
}
