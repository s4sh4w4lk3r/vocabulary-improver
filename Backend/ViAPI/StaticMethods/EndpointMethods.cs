using System.Collections.Generic;
using ViAPI.Auth;
using ViAPI.Database;
using ViAPI.Entites.DTO;
using ViAPI.Entities;

namespace ViAPI.StaticMethods;

public static class EndpointMethods
{
    public static List<ViDictionaryDto> GetDicts(HttpContext context, ViDbContext db)
    {
        if (Accounting.IsContextHasGuid(context, out Guid guid) is true)
        {
            var dicts = db.GetDictionariesByUser(guid)!;
            var dtoDicts = new List<ViDictionaryDto>();
            foreach (var item in dicts)
            {
                dtoDicts.Add(new ViDictionaryDto(item.Name, item.Guid));
            }
            return dtoDicts;
        }
        else
        {
            return new List<ViDictionaryDto>();
        }
    }
}
