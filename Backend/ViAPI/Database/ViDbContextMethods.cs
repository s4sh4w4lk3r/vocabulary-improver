using ViAPI.Entities;
using ViAPI.StaticMethods;

namespace ViAPI.Database;

public partial class ViDbContext
{

    //ДбСеты, логгеры и тд объявлено в другом файле этого класса.

    #region Методы на добавление.
    public RegistredUser AddRegistredUser(string username, string email, string firstname, string password)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        RegistredUser user = new(Guid.NewGuid(), firstname, username, email, password);
        Users.Add(user);
        SaveChanges();
        Logger.LogInformation($"Method {methodName}, Status OK. User {user.Guid} added.");
        return user;
    }

    public TelegramUser AddTelegramUser(uint telegramId, string firstname)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        TelegramUser user = new(Guid.NewGuid(), firstname, telegramId);
        Users.Add(user);
        SaveChanges();
        Logger.LogInformation($"Method {methodName}, Status OK. User {user.Guid} added.");
        return user;
    }

    public ViDictionary? AddDictionary(string name, Guid userGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (InputChecker.CheckGuid(userGuid) is false)
        {
            Logger.LogWarning($"Method {methodName}, Status FAIL. Guid is empty.");
            return null;
        }

        User? user = Users.Find(userGuid);

        if (user is not null)
        {
            ViDictionary dict = new(Guid.NewGuid(), name, user.Guid);
            Dictionaries.Add(dict);
            SaveChanges();
            Logger.LogInformation($"Method {methodName}, Status: OK. Dictionary {dict.Guid} added.");
            return dict;
        }
        else
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. User {userGuid} not found.");
            return null;
        }
    }

    public Word? AddWord(string sourceWord, string targetWord, Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (InputChecker.CheckGuid(dictGuid) is false)
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
            return null;
        }

        ViDictionary? dict = Dictionaries.Find(dictGuid);

        if (dict is not null)
        {
            Word word = new(Guid.NewGuid(), sourceWord, targetWord, dict.Guid);
            Words.Add(word);
            SaveChanges();
            Logger.LogInformation($"Method {methodName}, Status: OK. Word {word.Guid} added.");
            return word;
        }
        else
        {
            Logger.LogWarning($"Dictionary {dictGuid}, Status: Fail. Dict {dictGuid} not found.");
            return null;
        }
    }
    #endregion

    #region Методы на удаление.
    public void RemoveUser(Guid userGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (InputChecker.CheckGuid(userGuid) is false)
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
            return;
        }

        User? user = Users.Find(userGuid);

        if (user is not null)
        {
            Users.Remove(user);
            SaveChanges();
            Logger.LogInformation($"Method {methodName}, Status: OK. User {userGuid} removed, as well as his entire dictionaries and words.");
        }
        else
        {
            Logger.LogWarning($"Method: {methodName} Status: FAIL. User {userGuid} not found.");
        }
    }
    public void RemoveDictionary(Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (InputChecker.CheckGuid(dictGuid) is false)
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
            return;
        }

        ViDictionary? dict = Dictionaries.Find(dictGuid);

        if (dict is not null)
        {
            Dictionaries.Remove(dict);
            SaveChanges();
            Logger.LogInformation($"Method {methodName}, Status: OK. Dictionary {dictGuid} removed.");
        }
        else
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. Dictionary {dictGuid} not found.");
        }
    }
    public void RemoveWord(Guid wordGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (InputChecker.CheckGuid(wordGuid) is false)
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
        }

        Word? word = Words.Find(wordGuid);

        if (word is not null)
        {
            Words.Remove(word);
            SaveChanges();
            Logger.LogInformation($"Method {methodName}, Status: OK. Word {wordGuid} removed.");
        }
        else
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. Word {wordGuid} not found.");
        }
    }
    #endregion

    #region Методы на выборку.
    public Word? GetWord(Guid wordGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (InputChecker.CheckGuid(wordGuid) is false)
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
            return null;
        }

        Word? word = Words.Find(wordGuid);

        if (word is not null)
        {
            Logger.LogInformation($"Method {methodName}, Status: OK. Word found {wordGuid}");
        }
        else
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. Word {wordGuid} not found.");
        }

        return word;
    }
    public List<Word>? GetWordsByDict(Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (InputChecker.CheckGuid(dictGuid) is false)
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
            return null;
        }

        ViDictionary? dict = Dictionaries.Find(dictGuid);

        if (dict is not null)
        {
            Logger.LogInformation($"Method {methodName}, Status: OK. Dict {dictGuid} found, Count: {dict.Count}");
            return dict.Words.ToList();
        }
        else
        {
            Logger.LogWarning($"Method {methodName}, Status: Fail. Dict {dictGuid} not found.");
            return null;
        }
    }
    public List<ViDictionary>? GetDictionariesByUser(Guid userGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (InputChecker.CheckGuid(userGuid) is false)
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
            return null;
        }

        User? user = Users.Find(userGuid);

        if (user is not null)
        {
            Logger.LogInformation($"Method {methodName}, Status: OK. User {userGuid} found, Count: {user.Dictionaries.Count}");
            return user.Dictionaries.ToList();
        }
        else
        {
            Logger.LogWarning($"Method {methodName}, Status: Fail. User {userGuid} not found.");
            return null;
        }
    }
    public bool ValidateHash(string username, string hash)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (InputChecker.CheckString(username.ToLower(), hash) is false)
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. Bad input format.");
        }

        RegistredUser? user = Set<RegistredUser>().Where(x => x.Username == username)?.First();


        if (user is not null)
        {
            bool isValid = hash == user.Hash;

            Logger.LogInformation(message: $"Method {methodName}, Status: OK. Username \"{username}\" hash validation value is \"{isValid}\"");
            return isValid;
        }
        else
        {
            Logger.LogWarning($"Method {methodName}, Status: FAIL. User {username} not found.");
            return false;
        }
    }
    #endregion

    Добавить методы на изменение данных в бд.
}
