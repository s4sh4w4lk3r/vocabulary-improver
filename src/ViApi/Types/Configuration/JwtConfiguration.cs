using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ViApi.Types.Configuration;

public class JwtConfiguration
{
    public  string? JwtKey { get; init; }
    public string? Audience { get; init; }
    public string? Issuer { get; init; }
    public int TokenLifeTime { get; init; }
    public SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey!));
}
