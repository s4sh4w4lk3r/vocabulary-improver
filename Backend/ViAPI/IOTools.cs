using System.Text.RegularExpressions;

namespace ServerSide
{
    public static class IOTools
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

        public static bool CheckEmail(string address)
        {
            Regex validateEmailRegex = new("^\\S+@\\S+\\.\\S+$");
            if (!string.IsNullOrWhiteSpace(address) && validateEmailRegex.IsMatch(address))
            {
                return true;
            }
            else return false;
        }
    }
}
