namespace ViApi.Types.Users
{
    public class ApiUser : UserBase
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public ApiUser(Guid userGuid, string firstname) : base(userGuid, firstname)
        {
#error дописать тут
        }
    }
}
