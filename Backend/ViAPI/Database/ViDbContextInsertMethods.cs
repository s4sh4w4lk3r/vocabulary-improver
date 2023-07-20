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

    public TelegramUser? AddTelegramUser(ulong telegramId, string firstname)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;
        bool userExists = Set<TelegramUser>().Any(u=> u.TelegramId == telegramId);

        if (userExists is false) 
        {
            TelegramUser user = new(Guid.NewGuid(), firstname, telegramId);
            Users.Add(user);
            SaveChanges();
            Logger?.LogInformation($"Method {methodName}, Status OK. User {user.Guid} added.");
            return user;
        }
        Logger?.LogWarning($"Method {methodName}, Status OK. User tgid: {telegramId} already exists.");
        return null;
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

    public Word? AddWord(Guid userGuid, string sourceWord, string targetWord, Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        bool userGuidOk = userGuid.IsNotEmpty();
        bool dictGuidOk = dictGuid.IsNotEmpty();
        bool wordsOk = InputChecker.CheckString(sourceWord, targetWord) && sourceWord.Length < 255 && targetWord.Length < 255;
        bool affilationUserToDict = CheckAffiliationUserToDict(userGuid, dictGuid);

        if (userGuidOk && dictGuidOk && wordsOk && affilationUserToDict is true)
        {
            Word word = new Word(Guid.NewGuid(), sourceWord, targetWord, dictGuid, 0);
            Words.Add(word);
            SaveChanges();
            Logger?.LogInformation($"Method {methodName}, Status: OK. Word {word.Guid} added.");
            return word;
        }
        else
        {
            string bools = $"userGuidOk: {userGuidOk}, dictGuidOk: {dictGuidOk}, wordsOk: {wordsOk}, affilationUserToDict: {affilationUserToDict}.";
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. {bools}");
            return null!;
        }
    }
}
