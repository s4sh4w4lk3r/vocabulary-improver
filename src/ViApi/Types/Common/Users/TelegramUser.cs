namespace ViApi.Types.Common.Users;

public class TelegramUser : UserBase
{
    public long TelegramId { get; init; }
    private TelegramUser() { }
    public TelegramUser(Guid userGuid, string firstname, long telegramId) : base(userGuid, firstname)
    {
        TelegramId = telegramId;
    }
    public TelegramUser(Guid userGuid, string firstname, long? telegramId) : base(userGuid, firstname)
    {
        telegramId.ThrowIfNull("В конструктор TelegramUser передан telegramId, который null.");
        TelegramId = (long)telegramId;
    }
    public override string ToString() => $"[{GetType().Name}] Guid: {Guid}, TelegramId: {TelegramId}, Firstname: {Firstname}";
}
