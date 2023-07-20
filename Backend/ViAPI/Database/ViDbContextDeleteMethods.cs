﻿using ViAPI.Entities;
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
    public bool RemoveDictionary(Guid userGuid, Guid dictGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        ViDictionary? dict = Dictionaries.Where(e => e.Guid == dictGuid && e.UserGuid == userGuid).FirstOrDefault();

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
    public bool RemoveWord(Guid userGuid, Guid wordGuid)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        Word? word = Words.Where(e => e.Guid == wordGuid && e.Dictionary!.UserGuid == userGuid).FirstOrDefault();
        if (word is not null)
        {
            Words.Remove(word);
            SaveChanges();
            Logger?.LogInformation($"Method {methodName}, Status: OK. Word {wordGuid} removed.");
            return true;
        }
        else
        {
            Logger?.LogWarning($"Method {methodName}, Status: FAIL. Word {wordGuid} not found for user {userGuid}.");
            return false;
        }
    }
}
