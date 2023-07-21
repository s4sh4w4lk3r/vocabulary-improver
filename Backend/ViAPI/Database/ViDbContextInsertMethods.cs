using ViAPI.Entities;
using ViAPI.Other;

namespace ViAPI.Database;

public partial class ViDbContext
{
    public ViResult<RegistredUser> AddRegistredUser(string username, string email, string firstname, string password)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        bool userExists = Set<RegistredUser>().Any(u => u.Username == username || u.Email == email);
        if (userExists is false)
        {
            RegistredUser user = new(Guid.NewGuid(), firstname, username, email, password);
            Users.Add(user);
            SaveChanges();
            return new ViResult<RegistredUser>(ViResultTypes.Created, user, methodName, "RegUser added.");
        }
        else
        {
            return new ViResult<RegistredUser>(ViResultTypes.UserExists, null, methodName, $"RegUser with username: {username} or {email} exists.");
        }
    }

    public ViResult<TelegramUser> AddTelegramUser(ulong telegramId, string firstname)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        bool userExists = Set<TelegramUser>().Any(u=> u.TelegramId == telegramId);
        if (userExists is false) 
        {
            TelegramUser user = new(Guid.NewGuid(), firstname, telegramId);
            Users.Add(user);
            SaveChanges();
            return new ViResult<TelegramUser>(ViResultTypes.Created, user, methodName, "TelegramUser added.");
        }
        else
        {
            return new ViResult<TelegramUser>(ViResultTypes.UserExists, null, methodName, $"TelegramUser with {telegramId} exists.");
        }
    }

    public ViResult<ViDictionary> AddDictionary(string name, Guid userGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        User? user = Users.Where(e => e.Guid == userGuid).FirstOrDefault();
        if (user is not null)
        {
            ViDictionary dict = new(Guid.NewGuid(), name, user.Guid);
            Dictionaries.Add(dict);
            SaveChanges();
            string message = $"Dictionary {dict.Guid} added.";
            return new ViResult<ViDictionary>(ViResultTypes.Created, dict, methodName, message);
        }
        else
        {
            string message = $"User {userGuid} not found.";
            return new ViResult<ViDictionary>(ViResultTypes.NotFoundDb, null, methodName, message);
        }
    }

    public ViResult<Word> AddWord(Guid userGuid, string sourceWord, string targetWord, Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        bool affilationUserToDict = CheckAffiliationUserToDict(userGuid, dictGuid);
        if (affilationUserToDict is true)
        {
            Word word = new Word(Guid.NewGuid(), sourceWord, targetWord, dictGuid, 0);
            Words.Add(word);
            SaveChanges();
            string message = $"Method {methodName}, Status: OK. Word {word.Guid} added.";
            return new ViResult<Word>(ViResultTypes.Created, word, methodName, message);
        }
        else
        {
            string message = $"Method {methodName}, Status: FAIL. The dictionary {dictGuid} does not belong to the user {userGuid}.";
            return new ViResult<Word>(ViResultTypes.NotFoundOrNoAffilationDb, null, methodName, message);
        }
    }
}
