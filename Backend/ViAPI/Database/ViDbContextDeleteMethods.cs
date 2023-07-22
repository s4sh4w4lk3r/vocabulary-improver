using ViAPI.Entities;
using ViAPI.Other;

namespace ViAPI.Database;

public partial class ViDbContext
{
    private bool RemoveUser(Guid userGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (userGuid.IsNotEmpty() is false)
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
            return false;
        }

        User? user = Users.Where(e => e.Guid == userGuid).FirstOrDefault();

        if (user is not null)
        {
            Users.Remove(user);
            SaveChanges();
            Logger?.LogInformation($"Method {methodName}, Status: OK. User {userGuid} removed, as well as his entire dictionaries and words.");
            return true;
        }
        else
        {
            Logger?.LogWarning($"Method: {methodName} Status: FAIL. User {userGuid} not found.");
            return false;
        }
    }
    public ViResult<ViDictionary> RemoveDictionary(Guid userGuid, Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        ViDictionary? dict = Dictionaries.Where(e => e.Guid == dictGuid && e.UserGuid == userGuid).FirstOrDefault();

        if (dict is not null)
        {
            Dictionaries.Remove(dict);
            SaveChanges();
            string message = $"Dictionary {dictGuid} removed.";
            return new ViResult<ViDictionary>(ViResultTypes.Removed, dict, methodName, message);
        }
        else
        {
            string message = $"Dictionary {dictGuid} with {userGuid} not found.";
            return new ViResult<ViDictionary>(ViResultTypes.NotFoundOrNoAffilationDb, null, methodName, message);
        }
    }
    public ViResult<Word> RemoveWord(Guid userGuid, Guid wordGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        Word? word = Words.Where(e => e.Guid == wordGuid && e.Dictionary!.UserGuid == userGuid).FirstOrDefault();
        if (word is not null)
        {
            Words.Remove(word);
            SaveChanges();
            string message = $"Dictionary {wordGuid} removed.";
            return new ViResult<Word>(ViResultTypes.Removed, word, methodName, message);
        }
        else
        {
            string message = $"Word {wordGuid} with {userGuid} not found.";
            return new ViResult<Word>(ViResultTypes.NotFoundOrNoAffilationDb, null, methodName, message);
        }
    }
}
