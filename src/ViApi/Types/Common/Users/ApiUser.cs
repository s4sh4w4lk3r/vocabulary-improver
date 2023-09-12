namespace ViApi.Types.Common.Users
{
    public class ApiUser : UserBase
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }

        private ApiUser() { }
        public ApiUser(Guid userGuid, string firstname, string username, string email, string password) : base(userGuid, firstname)
        {
            Username = username;
            Email = email;
            Password = password;
        }

        public override string ToString() => $"[{GetType().Name}] Guid: {Guid}, Username: {Username}, Email: {Email}, Firstname: {Firstname}";
    }
}
