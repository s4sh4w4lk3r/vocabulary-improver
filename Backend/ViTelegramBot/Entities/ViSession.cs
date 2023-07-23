using System.IdentityModel.Tokens.Jwt;

namespace ViTelegramBot.Entities;

public class ViSession
{
    public long TelegramId { get; }
    public string JwtToken { get; }
    public DateTime TokenExpiration { get; private set; }

    public ViSession(long telegramId, string jwtToken)
    {
        TelegramId = telegramId;
        JwtToken = jwtToken;
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
