namespace ViAPI.Entities;

public class TelegramUser : User
{
    public ulong TelegramId { get; set; }

    public TelegramUser(Guid guid, string firstname, ulong telegramId) : base(guid, firstname)
    {
        TelegramId = telegramId;
    }

    public override string ToString() => $"{base.ToString()}, TelegramID: {TelegramId}";
}
