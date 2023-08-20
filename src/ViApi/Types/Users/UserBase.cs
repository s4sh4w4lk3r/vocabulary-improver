using Throw;

namespace ViApi.Types.Users;

public abstract class UserBase
{
    public Guid Guid { get; set; }
    public string Firstname { get; set; }
    public UserBase(Guid userGuid, string firstname)
    {
        userGuid.Throw("В конструктор UserBase передан пустой GUID.").IfEquals(Guid.Empty);
        firstname.Throw("В конструктор UserBase передано пустое имя.").IfNullOrWhiteSpace(f => f);
        Firstname = firstname;
        Guid = userGuid;
    }
    public override string ToString() => $"[{GetType().Name}] Guid: {Guid}, Firstname: {Firstname}.";
}
