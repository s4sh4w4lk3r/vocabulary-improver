using ViAPI.Entities;

namespace ViAPI.Database;

public partial class ViDbContext
{
    public User? SaveEditedUser(User user)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        try
        {
            if (user is TelegramUser tguser)
            {
                tguser = new TelegramUser(tguser.Guid, tguser.Firstname, tguser.TelegramId);
                Users.Update(tguser);
                SaveChanges();
                Logger?.LogInformation($"Method {methodName}, Status OK. TelegramUser {tguser.Guid} updated.");
                return tguser;
            }
            else if (user is RegistredUser reguser)
            {
                reguser = new RegistredUser(reguser.Guid, reguser.Firstname, reguser.Username, reguser.Email, reguser.Hash);
                Users.Update(reguser);
                SaveChanges();
                Logger?.LogInformation($"Method {methodName}, Status: OK. RegistredUser {reguser.Guid} updated.");
                return reguser;
            }
            Logger?.LogWarning($"Method: {methodName}, Status FAIL. User downcast fail.");
            return null;
        }

        catch
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Some user properties is null.");
            return null;
        }
    }

    public ViDictionary? SaveEditedDictionary(ViDictionary dict)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        try
        {
            if (dict is not null)
            {
                dict = new ViDictionary(dict.Guid, dict.Name, dict.UserGuid);
                Dictionaries.Update(dict);
                SaveChanges();
                Logger?.LogInformation($"Method {methodName}, Status: OK. Dictionary {dict.Guid} updated.");
                return dict;
            }
            else
            {
                Logger?.LogWarning($"Method {methodName}, Status: FAIL. Dict is null.");
                return null;
            }
        }
        catch
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Some dict properties is null.");
            return null;
        }
    }
    public Word? SaveEditedWord(Word word)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        try
        {
            if (word is not null)
            {
                word = new Word(word.Guid, word.SourceWord, word.TargetWord, word.DictionaryGuid, word.Rating);
                Words.Update(word);
                SaveChanges();
                Logger?.LogInformation($"Method {methodName}, Status: OK. Word {word.Guid} updated.");
                return word;
            }
            else
            {
                Logger?.LogWarning($"Method {methodName}, Status: FAIL. Word is null.");
                return null;
            }
        }
        catch
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Some word properties is null.");
            return null;
        }

    }
    public enum RatingAction
    {
        Increase, Decrease
    }
    public async Task<Word?> UpdateRatingDbAsync(Guid wordGuid, RatingAction action)
    {
#warning проверка на принадлежность нужна
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        var word = Words.Where(e => e.Guid == wordGuid).FirstOrDefault();
        if (word is not null)
        {
            if (action is RatingAction.Increase) { word.IncreaseRating(); }
            if (action is RatingAction.Decrease) { word.DecreaseRating(); }

            Task saveTask = SaveChangesAsync();

            Logger?.LogInformation($"Method {methodName}, Status: OK. Word {wordGuid} updated.");
            await saveTask;
            return word;
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: Fail. Word {wordGuid} not found.");
            return null;
        }
    }
}
