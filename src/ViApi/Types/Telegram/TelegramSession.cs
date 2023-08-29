using MongoDB.Bson.Serialization.Attributes;
using ViApi.Types.Common;
using ViApi.Types.Common.Users;
using ViApi.Types.Enums;

namespace ViApi.Types.Telegram;

[BsonIgnoreExtraElements]
public class TelegramSession
{
    private Guid _userGuid;
    public Guid UserGuid
    {
        get => _userGuid;
        init => _userGuid = value.Throw("В конструктор получен пустой userGuid").IfDefault().Value;
    }
    public Guid DictionaryGuid {get; set;}
    public Stack<Word>? GameStack { get; init; }
    public UserState State { get; set; }
    public int MessageIdToEdit { get; init; }
    public long TelegramId { get; init; }

    private TelegramSession() { }
    public TelegramSession(TelegramUser user, UserState state = UserState.Default)
    {
        UserGuid = user.Guid;
        State = state;
        TelegramId = user.TelegramId;
    }

    public override string ToString() => $"UserGuid: {UserGuid}, TelegramId: {TelegramId}, Selected DictGuid: {DictionaryGuid} State: {State}";
}
