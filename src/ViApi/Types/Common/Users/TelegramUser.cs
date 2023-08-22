namespace ViApi.Types.Common.Users;

public class TelegramUser : UserBase
{
    private long telegramId;
    public long TelegramId
    {
        get => telegramId;
        set => telegramId = value.Throw("В конструктор TelegramUser передан telegramId, который меньше нуля.").IfNegativeOrZero().Value;
    }
    public TelegramUser() { }
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
