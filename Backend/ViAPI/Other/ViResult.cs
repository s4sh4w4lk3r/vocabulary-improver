namespace ViAPI.Other;

public class ViResult<T>
{
    public StatusType Status { get; private set; }
    public string Info { get; private set; }
    public T? ResultValue { get; private set; }

    public ViResult(StatusType status, T? value, string info = "")
    {
        Status = status;
        ResultValue = value;
        Info = info;
    }
    public enum StatusType { Ok, Fail}
}
