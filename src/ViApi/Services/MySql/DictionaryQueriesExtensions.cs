using Microsoft.EntityFrameworkCore;
using ViApi.Types.Common.Users;

namespace ViApi.Services.MySql;

public static class DictionaryQueriesExtensions
{
    public static async Task<bool> InsertDictionaryAsync(this MySqlDbContext db, UserBase user, string name)
    {
        user.ThrowIfNull("В метод InsertDictionaryAsync пришел null user.").IfDefault(g => g.Guid, "В метод InsertDictionaryAsync пришел пустой Guid.");
        name.Throw("В метод InsertDictionaryAsync пришло пустое имя").IfNullOrWhiteSpace(n => n);
        bool userExists = await db.Users.AnyAsync(u=>u.Guid == user.Guid);
        if (userExists is false) { return false; }

        await db.Dictionaries.AddAsync(new Types.Common.Dictionary(Guid.NewGuid(), name, user.Guid));
        int stringsChanged = await db.SaveChangesAsync();

        if (stringsChanged == 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
