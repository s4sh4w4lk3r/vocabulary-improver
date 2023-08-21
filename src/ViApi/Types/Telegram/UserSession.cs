using Throw;
using ViApi.Types.Common;
using ViApi.Types.Common.Users;
using ViApi.Types.Enums;

namespace ViApi.Types.Telegram;

public class UserSession
{
    public TelegramUser User { get; set; }
    public Dictionary Dictionary { get; set; }
    public Stack<Word> GameStack{ get; set; }
    public UserState State { get; set; }

    public UserSession(TelegramUser user, Dictionary dictionary, Stack<Word> gameStack, UserState state)
    {
        user.ThrowIfNull("В конструктор получен null user");
        dictionary.ThrowIfNull("В конструктор получен null dictionary");
        gameStack.ThrowIfNull("В конструктор получен null gameStack");

        User = user;
        Dictionary = dictionary;
        GameStack = gameStack;
        State = state;
    }

    public override string ToString() => $"[{GetType()}] TelegramId: {User.TelegramId}, State: {State}.";
}
