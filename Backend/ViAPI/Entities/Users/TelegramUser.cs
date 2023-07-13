namespace ViAPI.Entities;

public class TelegramUser : User
{
    public ulong Id { get; set; }

    public TelegramUser(Guid guid, string firstname, ulong id) : base(guid, firstname)
    {
        Id = id;
    }

    public override string ToString() => $"{base.ToString()}, TelegramID: {Id}";
}
