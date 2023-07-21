using ViAPI.Entites.DTO;
using ViAPI.Other;

namespace ViAPI.Database;

public partial class ViDbContext
{
    public ViResult<List<ViDictionaryDto>> GetDictionariesDtoByUser(Guid userGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        var dictsDto = Dictionaries.Where(p => p.UserGuid == userGuid).Select(p => new ViDictionaryDto(p.Guid, p.Name)).ToList();

        if (dictsDto is not null && dictsDto.Count > 0)
        {
            string message = $"User {userGuid} found, Count: {dictsDto.Count}";
            return new ViResult<List<ViDictionaryDto>>(ViResultTypes.Founded, dictsDto, methodName, message);
        }
        else
        {
            string message = $"Fail. User {userGuid} dicts not found.";
            return new ViResult<List<ViDictionaryDto>>(ViResultTypes.NotFoundDb, null, methodName, message);
        }
    }
    public ViResult<List<WordDto>> GetWordsDtoByDict(Guid userGuid, Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        List<WordDto>? wordsDto = Words.Where(p => p.DictionaryGuid == dictGuid && p.Dictionary!.UserGuid == userGuid).Select(p => new WordDto(p.Guid, p.SourceWord, p.TargetWord, p.Rating)).ToList();

        if (wordsDto is not null && wordsDto.Count > 0)
        {
            string message = $"Dict {dictGuid} found, Count: {wordsDto.Count}";
            return new ViResult<List<WordDto>>(ViResultTypes.Founded, wordsDto, methodName, message);
        }
        else
        {
            string message = $"The combination of User {userGuid} and dict {dictGuid} not found.";
            return new ViResult<List<WordDto>>(ViResultTypes.NotFoundDb, null, methodName, message);
        }
    }
}
