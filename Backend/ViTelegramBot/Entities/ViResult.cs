namespace ViTelegramBot.Entities;

public class ViResult<T>
{
    public ViResultTypes ResultCode { get; private set; }
    public T? ResultValue { get; private set; }
    public string MethodName { get; set; }
    public string Message { get; set; }
    public ViResult(ViResultTypes code, T? value, string methodName, string message = "")
    {
        ResultCode = code;
        ResultValue = value;
        MethodName = methodName;
        Message = message;
    }
    public override string ToString() => $"Code: {ResultCode}({(int)ResultCode}), Value: {ResultValue}, Method: {MethodName}, Message: {Message}.";
}
public enum ViResultTypes
{
    Founded, 
    Created, Removed, Updated,
    Fail
}
