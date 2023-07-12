using System.Text.RegularExpressions;

namespace ViAPI.StaticMethods
{
    public static class InputChecker
    {
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
        public static bool CheckEmail(string email)
        {
            Regex validateEmailRegex = new("^\\S+@\\S+\\.\\S+$");
            if (!string.IsNullOrWhiteSpace(email) && validateEmailRegex.IsMatch(email))
            {
                return true;
            }
            else return false;
        }
        public static bool CheckRating(int rating)
        {
            if (rating >= 0 && rating <= 10)
            {
                return true;
            }
            else return false;
        }
        public static void CheckStringException(params string[] strings)
        {
            if (CheckString(strings) is false)
            {
                throw new ArgumentException("The string consists of a space, empty or NULL.");
            }
        }
        public static void CheckEmailException(string email)
        {
            if (CheckEmail(email) is false)
            {
                throw new ArgumentException("The Email string has an invalid format or NULL.");
            }
        }
        public static void CheckRatingException(int rating)
        {
            if (CheckRating(rating) is false)
            {
                throw new ArgumentException("The rating value is not in the range from 0 to 10 inclusive.");
            }
        }
    }
}
