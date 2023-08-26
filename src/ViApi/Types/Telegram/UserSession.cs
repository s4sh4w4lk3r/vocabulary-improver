using MongoDB.Bson.Serialization.Attributes;
using ViApi.Types.Common;
using ViApi.Types.Enums;

namespace ViApi.Types.Telegram;

[BsonIgnoreExtraElements]
public class UserSession
{
    private Guid _userGuid;
    private Guid _dictionaryGuid;
    private Stack<Word> _gameStack = null!;

    public Guid UserGuid
    {
        get => _userGuid;
        init => _userGuid = value.Throw("В конструктор получен пустой userGuid").IfDefault().Value;
    }
    public Guid DictionaryGuid
    {
        get => _dictionaryGuid;
        init => _dictionaryGuid = value.Throw("В конструктор получен пустой dictGuid").IfDefault().Value;
    }
    public Stack<Word> GameStack
    {
        get => _gameStack;
        init => _gameStack = value.ThrowIfNull("В конструктор получен null gameStack").Value;
    }
    public UserState State { get; init; }
    public int LastMessageId { get; init; }

    private UserSession() { }
    public UserSession(Guid userGuid, Guid dictGuid, Stack<Word> gameStack, UserState state = UserState.Default)
    {
        UserGuid = userGuid;
        DictionaryGuid = dictGuid;
        GameStack = gameStack;
        State = state;
    }

    public override string ToString() => $"[{GetType()}] TGUserGuid: {UserGuid}, State: {State}";
}
