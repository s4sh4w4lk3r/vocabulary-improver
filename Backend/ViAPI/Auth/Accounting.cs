using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ViAPI.StaticMethods;

namespace ViAPI.Auth;

public static class Accounting
{
#warning    ебануть логгера сюда ко всем методам
    static ILogger Logger { get; set; }

    static Accounting()
    {
        Logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger(typeof(EndpointMethods).Name);
    }

    #region JWT.
    public static string GenerateJwt(Guid guid, int minutes = 10)
    {
        var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, guid.ToString()) };
        var jwt = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(minutes)),
                signingCredentials: new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    public static JwtBearerOptions GetJwtOptions(JwtBearerOptions options)
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = Issuer,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = Audience,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true
        };
        return options;
    }
    private static string Issuer { get; } = "ViTokenIssuer";
    private static string Audience { get; } = "ViUserAudience";
    private static string Key { get; } = ViConfiguration.GetSecretString(ViConfiguration.SecretType.JWTKey);
    private static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
    #endregion

    #region BCryptHashing.
    public static string GenerateHash(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    public static bool VerifyHash(string password, string hash) => BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
    #endregion

    #region GUID
    public static bool TryGetGuidFromContext(HttpContext http, out Guid guid)
    {
        string? claimValue = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(claimValue) is false)
        {
            guid = Guid.Parse(claimValue);
            return true;
        }
        else
        {
            guid = Guid.Empty;
            return false;
        }
    }
    #endregion
}

