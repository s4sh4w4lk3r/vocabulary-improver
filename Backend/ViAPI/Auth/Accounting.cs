using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ViAPI.Database;
using ViAPI.Entities;
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
    private static string Issuer { get; } = "ViTokenIssuer";
    private static string Audience { get; } = "ViUserAudience"; 

    private static string key = ViConfiguration.GetSecretString(ViConfiguration.SecretType.JWTKey);
    private static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

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
    public static bool IsContextHasGuid(HttpContext context, out Guid guid)
    {
        var claimValue = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

    public static string GenerateHash(string password) => BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    public static bool ValidateHash(string password, string hash) => BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
    #region Telegram.
    public static string TelegramLoginHandler(string idString, ViDbContext db, HttpContext context)
    {
        string methodName = System.Reflection.MethodBase.GetCurrentMethod()!.Name;

        if (ulong.TryParse(idString, out ulong id))
        {
            if (db.IsTelegramUserExists(id, out Guid guid) is true) 
            {
                Logger?.LogInformation($"Method {methodName}, Status OK. TelegramUser {id} has guid {guid}.");
                context.Response.StatusCode = 200;
                return GenerateJwt(guid);
            }
            else
            {
                string errorMessage = $"TelegramUser {id} not found.";
                Logger?.LogWarning($"Method {methodName}, Status FAIL. {errorMessage}");
                context.Response.StatusCode = 400;
                return errorMessage;
            }
        }
        else
        {
            string errorMessage = $"Bad input.";
            Logger?.LogWarning($"Method {methodName}, Status FAIL. {errorMessage}");
            context.Response.StatusCode = 400;
            return errorMessage;
        }
    }
    #endregion
}

