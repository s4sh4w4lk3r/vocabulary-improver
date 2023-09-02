using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using ViApi.Types.Common;

namespace ViApi.Services.Repository;

public partial class TgRepository : IRepository
{
    private readonly MySqlDbContext _mysql;
    private readonly IMongoDatabase _mongo;

    public TgRepository(MySqlDbContext mysql, IMongoDatabase mongo)
    {
        _mysql = mysql;
        _mongo = mongo;
    }

    public async Task<bool> CheckDictionaryIsExistAsync(Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default)
    {
        return (await _mysql.Dictionaries.Where(d => d.Guid == dictGuid && d.UserGuid == userGuid).CountAsync(cancellationToken: cancellationToken)) == 1;
    }
    public async Task<bool> DeleteDictionaryAsync(Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод DeleteDictionaryAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод DeleteDictionaryAsync пришел пустой dictGuid").IfDefault();

        var dictToDel = await _mysql.Dictionaries.Where(d => d.Guid == dictGuid && d.UserGuid == userGuid).FirstOrDefaultAsync(cancellationToken);
        if (dictToDel is null) { return false; }

        _mysql.Dictionaries.Remove(dictToDel);
        await _mysql.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<List<Dictionary>> GetDicionariesList(Guid userGuid, CancellationToken cancellationToken = default)
    {
        return await _mysql.Dictionaries.Where(d => d.UserGuid == userGuid).ToListAsync(cancellationToken: cancellationToken);
    }
    public async Task<bool> InsertDictionaryAsync(Dictionary dictionary, CancellationToken cancellationToken = default)
    {
        dictionary.UserGuid.Throw("В метод InsertDictionaryAsync пришел пустой Guid").IfDefault();
        dictionary.Name.Throw("В метод InsertDictionaryAsync пришло пустое имя").IfNullOrWhiteSpace(n => n);
        bool userExists = await _mysql.Users.AnyAsync(u => u.Guid == dictionary.UserGuid, cancellationToken);
        if (userExists is false) { return false; }

        await _mysql.Dictionaries.AddAsync(dictionary, cancellationToken);
        int stringsChanged = await _mysql.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<bool> RenameDictionaryAsync(Guid userGuid, Guid dictGuid, string newName, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод RenameDictionaryAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод RenameDictionaryAsync пришел пустой dictGuid").IfDefault();
        newName.Throw("В метод RenameDictionaryAsync пришло пустое имя").IfNullOrWhiteSpace(n => n);
        var dictToRemove = await _mysql.Dictionaries.Where(d => d.Guid == dictGuid && d.UserGuid == userGuid).FirstOrDefaultAsync(cancellationToken);
        if (dictToRemove is null) { return false; }

        dictToRemove.Name = newName;
        await _mysql.SaveChangesAsync(cancellationToken);
        return true;
    }
}