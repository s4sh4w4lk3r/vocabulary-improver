using ViAPI.Entities;
using ViAPI.StaticMethods;

namespace ViAPI.Database;

public partial class ViDbContext
{
    private RegistredUser AddRegistredUser(string username, string email, string firstname, string password)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        RegistredUser user = new(Guid.NewGuid(), firstname, username, email, password);
        Users.Add(user);
        SaveChanges();
        Logger?.LogInformation($"Method {methodName}, Status OK. User {user.Guid} added.");
        return user;
    }

    private TelegramUser AddTelegramUser(uint telegramId, string firstname)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        TelegramUser user = new(Guid.NewGuid(), firstname, telegramId);
        Users.Add(user);
        SaveChanges();
        Logger?.LogInformation($"Method {methodName}, Status OK. User {user.Guid} added.");
        return user;
    }

    private ViDictionary? AddDictionary(string name, Guid userGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (userGuid.IsNotEmpty() is false)
        {
            Logger?.LogWarning($"Method {methodName}, Status FAIL. Guid is empty.");
            return null;
        }

        User? user = Users.Where(e => e.Guid == userGuid).FirstOrDefault();

        if (user is not null)
        {
            ViDictionary dict = new(Guid.NewGuid(), name, user.Guid);
            Dictionaries.Add(dict);
            SaveChanges();
            Logger?.LogInformation($"Method {methodName}, Status: OK. Dictionary {dict.Guid} added.");
            return dict;
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. User {userGuid} not found.");
            return null;
        }
    }

    private Word? AddWord(string sourceWord, string targetWord, Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (dictGuid.IsNotEmpty() is false)
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
            return null;
        }

        ViDictionary? dict = Dictionaries.Where(e => e.Guid == dictGuid).FirstOrDefault();

        if (dict is not null)
        {
            Word word = new(Guid.NewGuid(), sourceWord, targetWord, dict.Guid, 0);
            Words.Add(word);
            SaveChanges();
            Logger?.LogInformation($"Method {methodName}, Status: OK. Word {word.Guid} added.");
            return word;
        }
        else
        {
            Logger?.LogWarning($"Dictionary {dictGuid}, Status: Fail. Dict {dictGuid} not found.");
            return null;
        }
    }
}
