using System.Text.RegularExpressions;
using Throw;
using ViApi.Types.Common.Users;

namespace ViApi.Types.Users
{
    public partial class ApiUser : UserBase
    {
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        private ApiUser() { }
        public ApiUser(Guid userGuid, string firstname, string username, string email, string password) : base(userGuid, firstname)
        {
            username.Throw("В конструктор ApiUser передан пустой ник.").IfNullOrWhiteSpace(f => f);

            password.Throw("В конструктор ApiUser передан пустой пароль.").IfNullOrWhiteSpace(f => f)
                .Throw("В конструктор ApiUser передан слабый пароль.").IfNotMatches(PasswordRegex());

            email.Throw("В конструктор ApiUser передан пустой Email.").IfNullOrWhiteSpace(_ => _)
                .Throw("В конструктор ApiUser передан Email неверного формата.").IfNotMatches(EmailRegex());
            Username = username;
            Email = email;
            Password = password;
        }

        public override string ToString() => $"[{GetType().Name}] Guid: {Guid}, Username: {Username}, Email: {Email}, Firstname: {Firstname}.";


        [GeneratedRegex("^\\S+@\\S+\\.\\S+$")]
        private static partial Regex EmailRegex();

        [GeneratedRegex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        private static partial Regex PasswordRegex();
    }
}
