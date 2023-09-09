using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ViApi.Types.Configuration;

public class JwtConfiguration
{
    private string jwtKey = null!;
    private string audience = null!;
    private string issuer = null!;
    private int tokenLifeTime;

    public required string JwtKey { get => jwtKey; init => jwtKey = value.Throw().IfNullOrWhiteSpace(s => s).Value; }
    public required string Audience { get => audience; init => audience = value.Throw().IfNullOrWhiteSpace(s => s).Value; }
    public required string Issuer { get => issuer; init => issuer = value.Throw().IfNullOrWhiteSpace(s => s).Value; }
    public required int TokenLifeTime { get => tokenLifeTime; init => tokenLifeTime = value.Throw().IfDefault().Value; }
    public SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
}
