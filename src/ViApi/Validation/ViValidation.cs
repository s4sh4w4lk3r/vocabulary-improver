using Throw;

namespace ViApi.Validation;

public class ViValidation
{
    public static bool IsNotEmptyStrings(params string?[] strings)
    {
        bool stringsOk = true;
        foreach (var item in strings)
        {
            if (string.IsNullOrWhiteSpace(item))
            {
                return false;
            }
        }
        return stringsOk;
    }
}
