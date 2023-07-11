namespace ViAPI.Entities.Users
{
    public class TelegramUser : User
    {
        public ulong Id { get; set; }
        public override string ToString() => $"{base.ToString()}, TelegramID: {Id}";
        public TelegramUser(Guid guid, string firstname, ulong id) : base(guid, firstname)
        {
            Id = id;
        }
    }
}
