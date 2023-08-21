using Throw;

namespace ViApi.Types.Common.Users;

public class TelegramUser : UserBase
{
    public long TelegramId { get; set; }
    public TelegramUser(Guid userGuid, string firstname, long telegramId) : base(userGuid, firstname)
    {
        TelegramId = telegramId;
    }
    public TelegramUser(Guid userGuid, string firstname, long? telegramId) : base(userGuid, firstname)
    {
        telegramId.ThrowIfNull(_ => new ArgumentNullException("В конструктор User передан telegramId который null."));
        TelegramId = (long)telegramId;
    }
    public override string ToString() => $"[{GetType().Name}] Guid: {Guid}, TelegramId: {TelegramId}, Firstname: {TelegramId}.";
}
