using ViAPI.Database;

namespace ServerSide.Database;

public static class DbHandler
{/*
    public static bool CheckConnection(ViDbContext db)
    {
        db.Database.CanConnect();
    }

    public static IEnumerable<DictionaryVi>? GetDictsByUserGuid(Guid userGuid)
    {
        using var db = new VocabularyImproverContext();
        return db.Dictionaries.Where(x => x.UserGuidFk == userGuid).Include(x => x.Words);
    }
    public static IEnumerable<WordDB>? GetWordsByDictGuid(Guid dictGuid)
    {
        using var db = new VocabularyImproverContext();
        return db.Words.Where(x => x.DictionaryGuidFk == dictGuid);
    }
    public static void IncreaseRating(Guid wordGuid)
    {
        const int MAX_RATING = 10;
        using var db = new VocabularyImproverContext();
        WordDB? word = db.Words.Find(wordGuid);
        if (word is not null && word?.Rating < MAX_RATING)
        {
            word.Rating++;
            db.SaveChanges();
        }
    }
    public static void DecreaseRating(Guid wordGuid)
    {
        const int MIN_RATING = 0;
        using var db = new VocabularyImproverContext();
        WordDB? word = db.Words.Find(wordGuid);
        if (word is not null && word.Rating > MIN_RATING)
        {
            word.Rating--;
            db.SaveChanges();
        }
    }
    public static void AddWord(string sourceWord, string targetWord, Guid dictGuid)
    {
        if (!IOTools.CheckString(sourceWord, targetWord))
        {
            throw new ArgumentException("Bad source or target word.");
        }
        using var db = new VocabularyImproverContext();
        var newWord = new WordDB()
        {
            WordGuidPk = Guid.NewGuid(),
            SourceWord = sourceWord,
            TargetWord = targetWord,
            Rating = 0,
            DictionaryGuidFk = dictGuid
        };
        db.Words.Add(newWord);
        db.SaveChanges();
    }//OK
    public static bool EditWord(string sourceWord, string targetWord, Guid wordGuid)
    {
        if (IOTools.CheckString(sourceWord, targetWord))
        {
            throw new ArgumentException("Bad source or target word.");
        }
        var db = new VocabularyImproverContext();
        WordDB? wordToUpdate = db.Words.Find(wordGuid);
        if (wordToUpdate is not null)
        {
            wordToUpdate.SourceWord = sourceWord;
            wordToUpdate.TargetWord = targetWord;
            db.SaveChanges();
            return true;
        }
        else return false;
    }
    /// <returns>Вернет true, если слово найдено в базе и было удалено, иначе false</returns>
    public static bool RemoveWord(Guid wordGuid)
    {
        using var db = new VocabularyImproverContext();
        WordDB? wordToRemove = db.Words.Find(wordGuid);
        if (wordToRemove is not null)
        {
            db.Remove(wordToRemove);
            db.SaveChanges();
            return true;
        }
        else return false;
    }

    public static void AddDictionary(string name, Guid userGuid)//OK
    {
        if (!IOTools.CheckString(name))
        {
            throw new ArgumentException("Dict name incorrect.");
        }
        using var db = new VocabularyImproverContext();
        var newDict = new DictionaryVi()
        {
            DictionaryGuidPk = Guid.NewGuid(),
            Name = name,
            UserGuidFk = userGuid
        };
        db.Dictionaries.Add(newDict);
        db.SaveChanges();
    }
    public static bool RemoveDictionary(Guid dictGuid)
    {
        using var db = new VocabularyImproverContext();
        var dict = db.Dictionaries.Find(dictGuid);
        if (dict is not null)
        {
            db.Dictionaries.Remove(dict);
            db.SaveChanges();
            return true;
        }
        return false;
    }
    public static bool EditDictName(Guid dictGuid, string newName)
    {
        if (!IOTools.CheckString(newName))
        {
            throw new ArgumentException("Incorrect name.");
        }
        using var db = new VocabularyImproverContext();
        var dict = db.Dictionaries.Find(dictGuid);
        if (dict is not null)
        {
            dict.Name = newName;
            db.SaveChanges();
            return true;
        }
        return false;
    }
    /// <returns>Если почты и ника нет в бд, то регает и возвращает true, иначе false</returns>
    public static bool RegisterUser(string username, string password, string firstname, string email)
    {
        if (!IOTools.CheckString(username, password, firstname) || !IOTools.CheckEmail(email))
        {
            throw new ArgumentException("Bad username, pass, firstname or email.");
        }

        using var db = new VocabularyImproverContext();
        var exitstingUser = db.Users.Where(x => x.Email == email || x.Username == username).FirstOrDefault();

        if (exitstingUser == null)
        {
            User newUser = new()
            {
                UserGuidPk = Guid.NewGuid(),
                Username = username,
                Firstname = firstname,
                Email = email
            };
            Password newPassword = new()
            {
                Hash = Auth.Hash.GenerateHash(password),
                UserGuidFkNavigation = newUser
            };
            db.Users.Add(newUser);
            db.Passwords.Add(newPassword);
            db.SaveChanges();
            return true;
        }
        else return false;
    }

    public enum DataType { Email, Username, Firstname }
    public static void ChangeUserData(Guid userGuid, string toChange, DataType changeUserDataType)
    {
        if (!IOTools.CheckString(toChange))
        {
            throw new ArgumentException("To change string incorrect.");
        }
        using var db = new VocabularyImproverContext();
        User? userToChange = db.Users.Find(userGuid);

        if (userToChange is null)
        {
            return;
        }

        switch (changeUserDataType)
        {
            case DataType.Email:
                userToChange.Email = toChange;
                break;
            case DataType.Firstname:
                userToChange.Firstname = toChange;
                break;
            case DataType.Username:
                userToChange.Username = toChange;
                break;
        }
        db.SaveChanges();
    }*/
}

