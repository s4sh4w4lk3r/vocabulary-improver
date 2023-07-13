using ViAPI.StaticMethods;

namespace ViAPI.Entities;

public abstract class User
{
    public Guid Guid { get; set; }
    public string Firstname { get; set; } = string.Empty;

    public User(Guid guid, string firstname)
    {   
        InputChecker.CheckStringException(firstname);
        Guid = guid;
        Firstname = firstname;
    }

    public override string ToString() => $"[{GetType().Name}] Guid: {Guid}, Firstname: {Firstname}";
}
