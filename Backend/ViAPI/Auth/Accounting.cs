using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ViAPI.Other;

namespace ViAPI.Auth;

public static class Accounting
{
    #region JWT.
    public static string GenerateJwt(Guid guid, int minutes = 10)
    {
        if (guid.IsNotEmpty() is false) { throw new ArgumentException("Guid is empty.");  }
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
    public static ViResult<Guid> TryGetGuidFromHttpContext(HttpContext http)
    {
        string methodName = nameof(TryGetGuidFromHttpContext);

        string? claimValue = http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (claimValue is null) 
        {
            return new ViResult<Guid>(ViResultTypes.BadClaim, Guid.Empty, methodName, "Claim is null.");
        }

        bool guidOK = Guid.TryParse(claimValue, out Guid userGuid);
        if (guidOK is true)
        {
            return new ViResult<Guid>(ViResultTypes.GetGuidFromHttpOk, userGuid, methodName, "Guid is OK.");
        }
        else
        {
            return new ViResult<Guid>(ViResultTypes.BadGuid, Guid.Empty, methodName, "Bad guid parsing.");
        }
    }
    #endregion
}

