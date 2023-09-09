namespace ViApi.Types.API;

public class ViApiResponse<T> where T : notnull
{
    public bool Success { get; init; }
    public T? Value { get; init; }
    public string Description { get; init; }

    public ViApiResponse(T? value, bool success, string desciption)
    {
        value.ThrowIfNull("В конструктор ViApiResponse поступил null value.");
        desciption.ThrowIfNull().IfNullOrWhiteSpace(s => s);
        Value = value;
        Success = success;
        Description = desciption;
    }
}
