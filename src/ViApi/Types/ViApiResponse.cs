using Throw;

namespace ViApi.Types;

public class ViApiResponse<T> where T: notnull
{
    public bool Success { get; set; }
    public T Value { get; set; }

    public ViApiResponse(T value, bool success)
    {
        value.ThrowIfNull("В конструктор ViApiResponse поступил null value.");
        Value = value;
        Success = success;
    }
}
