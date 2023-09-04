namespace ViApi.Types.Common.Users;

public abstract class UserBase
{
    public Guid Guid { get; init; }
    public string? Firstname { get; init; }
    public List<Dictionary>? Dictionaries { get; init; }
    protected UserBase() { }
    public UserBase(Guid userGuid, string firstname)
    {
        Firstname = firstname;
        Guid = userGuid;
    }
    public override string ToString() => $"[{GetType().Name}] Guid: {Guid}, Firstname: {Firstname}";
}
