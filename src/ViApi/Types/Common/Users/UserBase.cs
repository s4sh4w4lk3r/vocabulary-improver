namespace ViApi.Types.Common.Users;

public abstract class UserBase
{
    private Guid _guid;
    private string _firstname = null!;
    public Guid Guid
    {
        get => _guid;
        set => _guid = value.Throw("В конструктор UserBase передан пустой GUID.").IfDefault().Value;
    }
    public string Firstname
    {
        get => _firstname;
        set => _firstname = value.Throw("В конструктор UserBase передано пустое имя.").IfNullOrWhiteSpace(f => f).Value;
    }
    public List<Dictionary>? Dictionaries { get; set; }
    protected UserBase() { }
    public UserBase(Guid userGuid, string firstname)
    {
        Firstname = firstname;
        Guid = userGuid;
    }
    public override string ToString() => $"[{GetType().Name}] Guid: {Guid}, Firstname: {Firstname}";
}
