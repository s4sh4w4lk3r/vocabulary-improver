using MongoDB.Bson.Serialization.Attributes;
using Throw;

namespace ViApi.Types.Common.Users;

public abstract class UserBase
{
    public Guid Guid { get; set; }
    public string Firstname { get; set; } = null!;
    [BsonIgnore] public List<Dictionary>? Dictionaries { get; set; }
    public UserBase() { }
    public UserBase(Guid userGuid, string firstname)
    {
        userGuid.Throw("В конструктор UserBase передан пустой GUID.").IfDefault();
        firstname.Throw("В конструктор UserBase передано пустое имя.").IfNullOrWhiteSpace(f => f);
        Firstname = firstname;
        Guid = userGuid;
    }
    public override string ToString() => $"[{GetType().Name}] Guid: {Guid}, Firstname: {Firstname}.";
}
