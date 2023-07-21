using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using ViAPI.Entities;
using ViAPI.Other;

namespace ViAPI.Database;

public partial class ViDbContext
{
    private User? SaveEditedUser(User user)
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

    public ViResult<ViDictionary> UpdateDictionaryNameDb(Guid userGuid, Guid dictGuid, string newName)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        
        var dict = Dictionaries.Where(e=>e.Guid == dictGuid && e.UserGuid == userGuid).FirstOrDefault();

        if (dict is not null)
        {
            dict.Name = newName;
            SaveChanges();
            string message = $"Method {methodName}, Status: OK. Dict {dictGuid} new name is {newName}.";
            return new ViResult<ViDictionary>(ViResultTypes.Updated, dict, methodName, message);
        }
        else
        {
            string message = $"Method {methodName}, Status: Fail. Dict {dictGuid} not found for user {userGuid}.";
            return new ViResult<ViDictionary>(ViResultTypes.NotFoundOrNoAffilationDb, null, methodName, message);
        }
    }
    private Word? SaveEditedWord(Word word)
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
    
    public ViResult<Word> UpdateWordRatingDb(Guid userGuid, Guid wordGuid, RatingAction action)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        Word? word = Words.Where(e => e.Guid == wordGuid && e.Dictionary!.UserGuid == userGuid).FirstOrDefault();

        if (word is not null)
        {
            if (action is RatingAction.Increase) { word.IncreaseRating(); }
            if (action is RatingAction.Decrease) { word.DecreaseRating(); }
            SaveChanges();

            string message = $"Word {wordGuid} updated.";
            return new ViResult<Word>(ViResultTypes.Updated, word, methodName, message);
        }
        else
        {
            string message = $"The combination of User {userGuid} and word {wordGuid} not found.";
            return new ViResult<Word>(ViResultTypes.NotFoundOrNoAffilationDb, word, methodName, message);
        }
    }
}
public enum RatingAction
{
    Increase, Decrease
}
