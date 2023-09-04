using System.Text.RegularExpressions;

namespace ViApi.Types.Common.Users
{
    public partial class ApiUser : UserBase
    {
        public string? Username { get; init; }
        public string? Email { get; init; }
        public string? Password { get; init; }

        private ApiUser() { }
        public ApiUser(Guid userGuid, string firstname, string username, string email, string password) : base(userGuid, firstname)
        {
            Username = username;
            Email = email;
            Password = password;
        }

        public override string ToString() => $"[{GetType().Name}] Guid: {Guid}, Username: {Username}, Email: {Email}, Firstname: {Firstname}";


        [GeneratedRegex("^\\S+@\\S+\\.\\S+$")]
        private static partial Regex EmailRegex();
    }
}
