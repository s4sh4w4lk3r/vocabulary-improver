using Microsoft.EntityFrameworkCore;
using ViApi.Types.Common;

namespace ViApi.Services.MySql;

public static class DictionaryQueriesExtensions
{
    public static async Task<bool> InsertDictionaryAsync(this MySqlDbContext db, Dictionary dictionary, CancellationToken cancellationToken = default)
    {
        dictionary.UserGuid.Throw("В метод InsertDictionaryAsync пришел пустой Guid").IfDefault();
        dictionary.Name.Throw("В метод InsertDictionaryAsync пришло пустое имя").IfNullOrWhiteSpace(n => n);
        bool userExists = await db.Users.AnyAsync(u => u.Guid == dictionary.UserGuid, cancellationToken);
        if (userExists is false) { return false; }

        await db.Dictionaries.AddAsync(dictionary, cancellationToken);
        int stringsChanged = await db.SaveChangesAsync(cancellationToken);
        return true;
    }
    public static async Task<bool> DeleteDictionaryAsync(this MySqlDbContext db, Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод DeleteDictionaryAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод DeleteDictionaryAsync пришел пустой dictGuid").IfDefault();

        var dictToDel = await db.Dictionaries.Where(d => d.Guid == dictGuid && d.UserGuid == userGuid).FirstOrDefaultAsync(cancellationToken);
        if (dictToDel is null) { return false; }

        db.Dictionaries.Remove(dictToDel);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
    public static async Task<bool> RenameDictionaryAsync(this MySqlDbContext db, Guid userGuid, Guid dictGuid, string newName, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод RenameDictionaryAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод RenameDictionaryAsync пришел пустой dictGuid").IfDefault();
        newName.Throw("В метод RenameDictionaryAsync пришло пустое имя").IfNullOrWhiteSpace(n => n);
        var dictToRemove = await db.Dictionaries.Where(d => d.Guid == dictGuid && d.UserGuid == userGuid).FirstOrDefaultAsync(cancellationToken);
        if (dictToRemove is null) { return false; }

        dictToRemove.Name = newName;
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
