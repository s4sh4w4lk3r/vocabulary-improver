using Microsoft.EntityFrameworkCore;
using ViApi.Types.Common;

namespace ViApi.Services.Repository;

public partial class TgRepository
{
    public async Task UpdateWordRating(Guid userGuid, Guid dictGuid, Guid wordGuid, RatingAction action, CancellationToken cancellationToken = default)
    {
        var word = await _mysql.Words.FirstOrDefaultAsync(w => w.Guid == wordGuid && w.Dictionary!.Guid == dictGuid && w.Dictionary.UserGuid == userGuid, cancellationToken);

        if (word is null) { return; }

        if (action is RatingAction.Increase)
        {
            word.IncreaseRating();
        }
        if (action is RatingAction.Increase)
        {
            word.DecreaseRating();
        }

        await _mysql.SaveChangesAsync();
    }
    public async Task<List<Word>?> GetWordsAsync(Guid userGuid, Guid dictGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод GetWordsAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод GetWordsAsync пришел пустой dictGuid").IfDefault();

        var dict = await _mysql.Dictionaries.Include(d => d.Words).FirstOrDefaultAsync(d => d.Guid == dictGuid && d.UserGuid == userGuid, cancellationToken);

        var words = dict?.Words;

        return words;
    }
    public async Task<bool> DeleteWordAsync(Guid userGuid, Guid dictGuid, Guid wordGuid, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод DeleteWordAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод DeleteWordAsync пришел пустой dictGuid").IfDefault();
        wordGuid.Throw("В метод DeleteWordAsync пришел пустой wordGuid").IfDefault();

        var wordToDel = await _mysql.Words.Include(w => w.Dictionary).FirstOrDefaultAsync(w => w.DictionaryGuid == dictGuid && w.Guid == wordGuid && w.Dictionary!.UserGuid == userGuid, cancellationToken);
        if (wordToDel is null) { return false; };

        _mysql.Words.Remove(wordToDel);
        await _mysql.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<bool> InsertWordAsync(Guid userGuid, Guid dictGuid, string sourceWord, string targetWord, CancellationToken cancellationToken = default)
    {
        userGuid.Throw("В метод InsertWordAsync пришел пустой userGuid").IfDefault();
        dictGuid.Throw("В метод InsertWordAsync пришел пустой dictGuid").IfDefault();
        sourceWord.Throw("В метод InsertDictionaryAsync пришло пустое sourceWord").IfNullOrWhiteSpace(n => n);
        targetWord.Throw("В метод InsertDictionaryAsync пришло пустое targetWord").IfNullOrWhiteSpace(n => n);

        var dictExist = await _mysql.Dictionaries.AnyAsync(d => d.Guid == dictGuid && d.UserGuid == userGuid, cancellationToken);
        if (dictExist is false) { return false; }

        await _mysql.Words.AddAsync(new Word(Guid.NewGuid(), sourceWord, targetWord, dictGuid), cancellationToken);
        await _mysql.SaveChangesAsync(cancellationToken);
        return true;
    }
}
