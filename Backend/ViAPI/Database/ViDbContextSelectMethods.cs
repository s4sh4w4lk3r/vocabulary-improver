using ViAPI.Auth;
using ViAPI.Entities;
using ViAPI.StaticMethods;

namespace ViAPI.Database;

public partial class ViDbContext
{
    private Word? GetWord(Guid wordGuid)
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
    private List<Word>? GetWordsByDict(Guid dictGuid)
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
    private List<ViDictionary>? GetDictionariesByUser(Guid userGuid)
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
    public bool IdentifyUser(string username, string password, out Guid userGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (InputChecker.CheckString(username, password) is false)
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Bad input format.");
        }

        username = username.ToLower();

        var user = Set<RegistredUser>().Where(e => e.Username == username).Select(u => new { u.Guid, u.Hash }).FirstOrDefault();

        if (user is not null)
        {
            bool hashIsValid = Accounting.VerifyHash(password, user.Hash);

            if (hashIsValid)
            {
                userGuid = user.Guid;
                Logger?.LogInformation(message: $"Method {methodName}, Status: OK. Username \"{username}\" has identified.");
                return true;
            }
        }

        userGuid = Guid.Empty;
        Logger?.LogInformation(message: $"Method {methodName}, Status: FAIL. Username \"{username}\" has not identified.");
        return false;
    }

    /// <summary>
    /// Проверяет есть ли тг пользователь в базе, если есть, вернет верный Guid.
    /// </summary>
    public bool TryGetGuidFromTgId(ulong id, out Guid guid)
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

    /// <summary>
    /// Проверяет принадлежит ли словарь пользователю.
    /// </summary>
    public bool CheckAffiliationUserToDict(Guid userGuid, Guid dictGuid)
    {

        InputChecker.CheckGuidException(userGuid);
        InputChecker.CheckGuidException(dictGuid);
        bool affiliation = Dictionaries.Any(d => d.Guid == dictGuid && d.UserGuid == userGuid);
        return affiliation;
    }

    /// <summary>
    ///Проверяет принадлежит ли слово пользователю.
    /// </summary>
    public bool CheckAffiliationUserToWord(Guid userGuid, Guid wordGuid)
    {
        InputChecker.CheckGuidException(userGuid);
        InputChecker.CheckGuidException(wordGuid);
        bool affiliation = Words.Any(d => d.Guid == wordGuid && d.Dictionary!.UserGuid == userGuid);
        return affiliation;
    }
}
