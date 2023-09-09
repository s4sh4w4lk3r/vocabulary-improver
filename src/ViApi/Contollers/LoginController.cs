using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using ViApi.Services.Repository;
using ViApi.Types.API;
using ViApi.Types.Common.Users;
using ViApi.Types.Configuration;

namespace ViApi.Contollers;

[Route("/api/login")]
[ApiController]
public class LoginController : ControllerBase
{
    readonly IRepository _repo;
    readonly JwtConfiguration _jwtConf;
    readonly CancellationToken _cancellationToken = default;
    public LoginController([FromServices] IRepository repo, IOptions<JwtConfiguration> jwtConf, CancellationToken cancellationToken = default)
    {
        _repo = repo;
        _jwtConf = jwtConf.Value;
        _cancellationToken = cancellationToken;
    }

    [Route("jwt")] 
    [HttpPost]
#warning испоользовать iresult
    public async Task<ViApiResponse<string>> LoginJwt(ApiUserDto apiUserDto)
    {
        var vaildUser = await GetValidUser(apiUserDto);
        if (vaildUser == null)
        {
            return new ViApiResponse<string>("", false, "Пользователь не найден");
        }

        if (CheckPassword(apiUserDto, vaildUser) is true)
        {
            return new ViApiResponse<string>(GetJwtToken(vaildUser), true, "Прохоидте");
        }
        else
        {
            return new ViApiResponse<string>("", false, "Неправильное имя пользователя или пароль");
        }
    }

    [Route("cookie")]
    [HttpPost]
    public async Task<string> LoginCookie(ApiUserDto apiUserDto)
    {
        var vaildUser = await GetValidUser(apiUserDto);
        if (vaildUser == null)
        {
            //return new ViApiResponse<ApiUserDto>(apiUserDto, false, "Пользователь не найден");
            return "user not found";
        }
        SetCookie(vaildUser);
        return "ok";
    }
    
    private static bool CheckPassword(ApiUserDto apiUserDto, ApiUser validUser)
        => BCrypt.Net.BCrypt.EnhancedVerify(apiUserDto.Password, validUser.Password);
    private async Task<ApiUser?> GetValidUser(ApiUserDto apiUserDto)
    {
        if ((string.IsNullOrWhiteSpace(apiUserDto.Email) is false) && (MailAddress.TryCreate(apiUserDto.Email, out MailAddress? email) is true) && email is not null)
        {
            var validUser = await _repo.GetValidUserAsync(email, _cancellationToken);
            if (validUser is not null)
            {
                return validUser;
            }
        }

        if (string.IsNullOrWhiteSpace(apiUserDto.Username) is false)
        {
            var validUser = await _repo.GetValidUserAsync(apiUserDto.Username, _cancellationToken);
            if (validUser is not null)
            {
                return validUser;
            }
        }
        return null;
    }
    private string GetJwtToken(ApiUser apiUser)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, apiUser.Guid.ToString())
        };

        var jwt = new JwtSecurityToken(
        issuer: _jwtConf.Issuer,
        audience: _jwtConf.Audience,
        claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(_jwtConf.TokenLifeTime)),
                signingCredentials: new SigningCredentials(_jwtConf.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    private async void SetCookie(ApiUser apiUser)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, apiUser.Guid.ToString())
        };

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
    }
}