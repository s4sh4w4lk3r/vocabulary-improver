using MongoDB.Bson.Serialization.Attributes;
using ViApi.Types.Common;
using ViApi.Types.Common.Users;
using ViApi.Types.Enums;

namespace ViApi.Types.Telegram;

[BsonIgnoreExtraElements]
public class TelegramSession
{
    private Guid _userGuid;
    private Stack<Word> _gameStack = null!;

    public Guid UserGuid
    {
        get => _userGuid;
        init => _userGuid = value.Throw("В конструктор получен пустой userGuid").IfDefault().Value;
    }
    public Guid DictionaryGuid {get; set;}
    public Stack<Word> GameStack
    {
        get => _gameStack;
        init => _gameStack = value.ThrowIfNull("В конструктор получен null gameStack").Value;
    }
    public UserState State { get; init; }
    public int MessageIdToEdit { get; init; }
    public long TelegramId { get; init; }

    private TelegramSession() { }
    public TelegramSession(TelegramUser user, UserState state = UserState.Default)
    {
        UserGuid = user.Guid;
        State = state;
        TelegramId = user.TelegramId;
    }

    public override string ToString() => $"[{GetType()}] TGUserGuid: {UserGuid}, State: {State}";
}
