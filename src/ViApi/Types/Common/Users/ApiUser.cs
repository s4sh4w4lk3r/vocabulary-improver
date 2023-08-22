using System.Text.RegularExpressions;
using ViApi.Types.Common.Users;

namespace ViApi.Types.Users
{
    public partial class ApiUser : UserBase
    {
        private string _username = null!;
        private string _email = null!;
        private string _password = null!;
        public string Username
        {
            get => _username;
            set => _username = value.Throw("В конструктор ApiUser передан пустой ник.").IfNullOrWhiteSpace(_ => _).Value;
        }
        public string Email
        {
            get => _email;
            set => _email = value.Throw("В конструктор ApiUser передан пустой Email.").IfNullOrWhiteSpace(_ => _)
                .Throw("В конструктор ApiUser передан Email неверного формата.").IfNotMatches(EmailRegex()).Value;
        }
        public string Password
        {
            get => _password;
            set => _password = value.Throw("В конструктор ApiUser передан пустой пароль.").IfNullOrWhiteSpace(f => f)
                .Throw("В конструктор ApiUser передан слабый пароль.").IfNotMatches(PasswordRegex()).Value;
        }

        public ApiUser() { }
        public ApiUser(Guid userGuid, string firstname, string username, string email, string password) : base(userGuid, firstname)
        {
            Username = username;
            Email = email;
            Password = password;
        }

        public override string ToString() => $"[{GetType().Name}] Guid: {Guid}, Username: {Username}, Email: {Email}, Firstname: {Firstname}";


        [GeneratedRegex("^\\S+@\\S+\\.\\S+$")]
        private static partial Regex EmailRegex();

        [GeneratedRegex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
        private static partial Regex PasswordRegex();
    }
}
