using Microsoft.EntityFrameworkCore;
using ViApi.Types.Common;

namespace ViApi.Services.MySql;

public static class WordQueriesExtensions
{
    public static async Task<bool> InsertWordAsync(this MySqlDbContext db, Guid userGuid, Guid dictGuid, string sourceWord, string targetWord, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод InsertWordAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод InsertWordAsync пришел пустой dictGuid").IfDefault();
        sourceWord.Throw("В метод InsertDictionaryAsync пришло пустое sourceWord").IfNullOrWhiteSpace(n => n);
        targetWord.Throw("В метод InsertDictionaryAsync пришло пустое targetWord").IfNullOrWhiteSpace(n => n);

        var dictExist = await db.Dictionaries.AnyAsync(d => d.Guid == dictGuid && d.UserGuid == userGuid, cancellationToken);
        if (dictExist is false) { return false; }

        await db.Words.AddAsync(new Word(Guid.NewGuid(), sourceWord, targetWord, dictGuid), cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public static async Task<bool> DeleteWordAsync(this MySqlDbContext db, Guid userGuid, Guid dictGuid, Guid wordGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод DeleteWordAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод DeleteWordAsync пришел пустой dictGuid").IfDefault();
        wordGuid.Throw("В метод DeleteWordAsync пришел пустой wordGuid").IfDefault();

        var wordToDel = await db.Words.Include(w => w.Dictionary).FirstOrDefaultAsync(w => w.DictionaryGuid == dictGuid && w.Guid == wordGuid && w.Dictionary!.UserGuid == userGuid, cancellationToken);
        if (wordToDel is null) { return false; };

        db.Words.Remove(wordToDel);
        await db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public static async Task<List<Word>?> GetWordsAsync(this MySqlDbContext db, Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод GetWordsAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод GetWordsAsync пришел пустой dictGuid").IfDefault();

        var dict = await db.Dictionaries.Include(d=>d.Words).FirstOrDefaultAsync(d => d.Guid == dictGuid && d.UserGuid == userGuid, cancellationToken);

        var words = dict?.Words;

        return words;

    }
}
