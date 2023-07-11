using ViAPI.Handlers;

namespace ViAPI.Entities.Users
{
    public class RegistredUser : User
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public RegistredUser(Guid guid, string firstname, string username, string email, string hash) : base(guid, firstname)
        {
            InputExceptions.CheckStringException(username, hash);
            InputExceptions.CheckEmailException(email);
            Username = username;
            Email = email;
            Hash = hash;
        }
        public override string ToString() => $"{base.ToString()}, Username: {Username}, Email: {Email}, Hash: {Hash}";
    }
}
