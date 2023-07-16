using System.Reflection;
using System.Text.RegularExpressions;

namespace ViAPI.StaticMethods;

public static class InputChecker
{
    #region Проверки.
    public static bool CheckString(params string[] strings)
    {
        foreach (var item in strings)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                return false;
            }
        }
        return true;
    }
    public static bool CheckRating(int rating)
    {
        if (rating >= 0 && rating <= 10)
        {
            return true;
        }
        else return false;
    }
    #endregion

    #region Проверки с исключениями.
    public static void CheckGuidException(Guid guid)
    {
        if (guid.IsNotEmpty() is false)
        {
            throw new ArgumentException("Empty Guid 00000000-0000-0000-0000-000000000000 encountered.");
        }
    }
    public static void CheckStringException(params string[] strings)
    {
        if (CheckString(strings) is false)
        {
            throw new ArgumentException("The string consists of a space, empty or NULL.");
        }
    }

    #endregion
}


public static class InputCheckerExtensions
{
    public static bool IsNotEmpty(this Guid guid) => guid != Guid.Empty;
    public static bool IsEmail(this string email)
    {
        Regex validateEmailRegex = new("^\\S+@\\S+\\.\\S+$");
        if (!string.IsNullOrWhiteSpace(email) && validateEmailRegex.IsMatch(email))
        {
            return true;
        }
        else return false;
    }
}
