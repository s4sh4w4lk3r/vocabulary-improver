using ViAPI.Entities;
using ViAPI.StaticMethods;

namespace ViAPI.Database;

public partial class ViDbContext
{

    //ДбСеты, логгеры и тд объявлено в другом файле этого класса.

    public bool CheckConnection() => Database.CanConnect();

    #region Методы на добавление. Возрващает добавленный объект, если всё прошло успешно, в противном случае вернет null.
    public RegistredUser AddRegistredUser(string username, string email, string firstname, string password)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        RegistredUser user = new(Guid.NewGuid(), firstname, username, email, password);
        Users.Add(user);
        SaveChanges();
        Logger?.LogInformation($"Method {methodName}, Status OK. User {user.Guid} added.");
        return user;
    }

    public TelegramUser AddTelegramUser(uint telegramId, string firstname)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        TelegramUser user = new(Guid.NewGuid(), firstname, telegramId);
        Users.Add(user);
        SaveChanges();
        Logger?.LogInformation($"Method {methodName}, Status OK. User {user.Guid} added.");
        return user;
    }

    public ViDictionary? AddDictionary(string name, Guid userGuid)
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

    public Word? AddWord(string sourceWord, string targetWord, Guid dictGuid)
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
    #endregion

    #region Методы на удаление. Возвращает true если запись была найдена и удалена, в противном случае вернет false.
    public bool RemoveUser(Guid userGuid)
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
    public bool RemoveDictionary(Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (dictGuid.IsNotEmpty() is false)
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
            return false;
        }

        ViDictionary? dict = Dictionaries.Where(e => e.Guid == dictGuid).FirstOrDefault();

        if (dict is not null)
        {
            Dictionaries.Remove(dict);
            SaveChanges();
            Logger?.LogInformation($"Method {methodName}, Status: OK. Dictionary {dictGuid} removed.");
            return true;
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Dictionary {dictGuid} not found.");
            return false;
        }
    }
    public bool RemoveWord(Guid wordGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (wordGuid.IsNotEmpty() is false)
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
            return false;
        }

        Word? word = Words.Where(e => e.Guid == wordGuid).FirstOrDefault();

        if (word is not null)
        {
            Words.Remove(word);
            SaveChanges();
            Logger?.LogInformation($"Method {methodName}, Status: OK. Word {wordGuid} removed.");
            return true;
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Word {wordGuid} not found.");
            return false;
        }
    }
    #endregion

    #region Методы на выборку. Вернет найденный объект, если он не был найден, то венется null.
    public Word? GetWord(Guid wordGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (wordGuid.IsNotEmpty() is false)
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
            return null;
        }

        Word? word = Words.Where(e => e.Guid == wordGuid).FirstOrDefault();

        if (word is not null)
        {
            Logger?.LogInformation($"Method {methodName}, Status: OK. Word found {wordGuid}");
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Word {wordGuid} not found.");
        }

        return word;
    }
    public List<Word>? GetWordsByDict(Guid dictGuid)
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
            Logger?.LogInformation($"Method {methodName}, Status: OK. Dict {dictGuid} found, Count: {dict.Count}");
            return dict.Words.ToList();
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: Fail. Dict {dictGuid} not found.");
            return null;
        }
    }
    public List<ViDictionary>? GetDictionariesByUser(Guid userGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (userGuid.IsNotEmpty() is false)
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Guid is empty.");
            return null;
        }

        User? user = Users.Where(e => e.Guid == userGuid).FirstOrDefault();

        if (user is not null)
        {
            Logger?.LogInformation($"Method {methodName}, Status: OK. User {userGuid} found, Count: {user.Dictionaries.Count}");
            return user.Dictionaries.ToList();
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: Fail. User {userGuid} not found.");
            return null;
        }
    }
    public bool ValidateHash(string username, string hash)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (InputChecker.CheckString(username, hash) is false)
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Bad input format.");
        }

        username = username.ToLower();

        bool isValid = Set<RegistredUser>().Any(e => e.Username == username && e.Hash == hash);

        Logger?.LogInformation(message: $"Method {methodName}, Status: OK. Username \"{username}\" hash validation value is \"{isValid}\"");

        return isValid;
    }
    #endregion

    #region Методы на сохранение изменений. Вернет объект, если объект для обновления был найден, в противном случае вернет null.
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
    public async Task<int> EditRatingDbAsync(Guid wordGuid, RatingAction action)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        var word = Words.Where(e => e.Guid == wordGuid).FirstOrDefault();
        if (word is not null)
        {
            if (action is RatingAction.Increase) { word.IncreaseRating(); }
            if (action is RatingAction.Decrease) { word.DecreaseRating(); }

            Task saveTask = SaveChangesAsync();

            Logger?.LogInformation($"Method {methodName}, Status: OK. Word {wordGuid} updated.");
            await saveTask;
            return word.Rating;
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: Fail. Word {wordGuid} not found.");
            return -1;
        }
    }
    #endregion
#warning        сюда тоже ебануть логгера
    public bool IsTelegramUserExists(ulong id, out Guid guid)
    {
        var user = Set<TelegramUser>().Where(u => u.TelegramId == id).FirstOrDefault();
        if (user is not null)
        {
            guid = user.Guid;
            return true;
        }
        else 
        { 
            guid = Guid.Empty;
            return false;
        }
    }
}
