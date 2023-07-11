using ViAPI.Auth;
using ViAPI.Handlers;

namespace ViAPI.Entities
{
    public abstract class User
    {
        public Guid Guid { get; set; }
        public string Firstname { get; set; }
        public override string ToString() => $"[User] Guid: {Guid}, Firstname: {Firstname}";
        public User(Guid guid, string firstname)
        {
            InputExceptions.CheckStringException(firstname);
            Guid = guid;
            Firstname = firstname;
        }
    }
}
