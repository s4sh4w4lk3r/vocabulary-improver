using System.IdentityModel.Tokens.Jwt;

namespace ViTelegramBot.Entities;

public class ViSession
{
    public long TelegramId { get; private set; }
    public string JwtToken { get; private set; }
    public DateTime TokenExpiration { get; private set; }
    public UserState State { get; set; }
    public Guid SelectedDictionaryGuid { get; set; }

    public ViSession(long telegramId, string jwtToken)
    {
        TelegramId = telegramId;
        JwtToken = jwtToken;
        State = UserState.Default;
        SetTokenExpiration();
    }

    public bool IsNotExpried()
    {
        DateTime currentDateTime = DateTime.UtcNow;
        if (TokenExpiration <= currentDateTime)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override string ToString() => $"TelegramId: {TelegramId}, IsNotExpired: {IsNotExpried()}, TokenExpiration: {TokenExpiration}, Token: \"{JwtToken}\".";
    private void SetTokenExpiration()
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(JwtToken);
        TokenExpiration = jwtSecurityToken.ValidTo;
    }
}
public enum UserState
{
    Default, ChoosingDict, ChoosingWord, Playing, DictSelected,
    AddingWord, AddingDict, DeletingWord, DeletingDict,
}
