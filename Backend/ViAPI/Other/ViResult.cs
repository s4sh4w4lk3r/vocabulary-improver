namespace ViAPI.Other;

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
    Created, Removed, Updated, Founded,
    BadGuid, GuidOK, GetGuidFromHttpOk, BadClaim,
    NotFoundDb, NotFoundOrNoAffilationDb, UserExists
}
