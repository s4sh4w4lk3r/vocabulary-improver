using MongoDB.Bson.Serialization.Attributes;
using Throw;
using ViApi.Types.Common;
using ViApi.Types.Enums;

namespace ViApi.Types.Telegram;

[BsonIgnoreExtraElements]
public class UserSession
{
    public Guid UserGuid { get; set; }
    public Guid DictionaryGuid { get; set; }
    public Stack<Word> GameStack { get; set; } = new Stack<Word>();
    public UserState State { get; set; }

    public UserSession(Guid userGuid, Guid dictGuid, Stack<Word> gameStack, UserState state = UserState.Default)
    {
        userGuid.Throw("В конструктор получен пустой userGuid").IfDefault();
        dictGuid.Throw("В конструктор получен пустой dictGuid").IfDefault();
        gameStack.ThrowIfNull("В конструктор получен null gameStack");

        UserGuid = userGuid;
        DictionaryGuid = dictGuid;
        GameStack = gameStack;
        State = state;
    }

    public override string ToString() => $"[{GetType()}] TGUserGuid: {UserGuid}, State: {State}";
}
