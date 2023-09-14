using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Threading;
using ViApi.Services.EmailService;
using ViApi.Services.Repository;
using ViApi.Types.API;
using ViApi.Types.Common.Users;
using ViApi.Types.Configuration;
using ViApi.Validation.Filters;
using ViApi.Validation.Fluent.DtoValidators;

namespace ViApi.Contollers.UserControllers;

[Route("/api/user")]
[ApiController]
public class UpdateUserContoller : ControllerBase
{
    private readonly IRepository _repo;
    private readonly JwtConfiguration _jwtConf;
    public UpdateUserContoller([FromServices] IRepository repository, [FromServices] IOptions<JwtConfiguration> jwtOptions)
    {
        _repo = repository;
        _jwtConf = jwtOptions.Value;
        _jwtConf.ThrowIfNull();
    }

    [Route("getapproval")]
    [HttpGet]
    [Authorize]
    [ValidateUser]

    public async Task<IActionResult> GetApprovalCodeToUpdateProfile([FromServices] IEmailClient emailClient, CancellationToken cancellationToken)
    {
        Guid userGuid = HttpContext.GetGuidOrDefaultFromRequest();
        var user = await _repo.GetValidUserAsync(userGuid, cancellationToken);
        if (user is null)
        {
            return BadRequest(new ViApiResponse<Guid>(userGuid, false, "Пользователь не найден"));
        }

        bool emailOk = MailAddress.TryCreate(user.Email, out MailAddress? userEmail);

        if ((emailOk is false) || (userEmail is null))
        {
            return BadRequest(new ViApiResponse<string>(user.Email, false, "Email имеет неверный формат"));
        }


        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, userGuid.ToString()),
            new Claim(ClaimTypes.Email, userEmail.ToString())
        };

        var jwt = new JwtSecurityToken(
            issuer: _jwtConf.Issuer,
            audience: _jwtConf.Audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(120)),
            signingCredentials: new SigningCredentials(_jwtConf.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var jwtToken = new JwtSecurityTokenHandler().WriteToken(jwt);

        bool mailIsSent = await emailClient.SendMessageAsync(jwtToken, userEmail);

        if (mailIsSent)
        {
            return Ok(new ViApiResponse<string>(CutEmail(userEmail), true, "Письмо отправлено на адрес в поле value"));
        }
        else
        {
            return Ok(new ViApiResponse<string>(CutEmail(userEmail), true, "Письмо почему-то не было отправлено"));
        }

        static string CutEmail(MailAddress email)
        {
            var emailStringBuilder = new StringBuilder(email.ToString());
            email.Throw().IfNullOrWhiteSpace(email => email.ToString());

            for (int i = 0; i < emailStringBuilder.Length / 3; i++)
            {
                emailStringBuilder[i] = '*';
            }
            return emailStringBuilder.ToString();
        }
    }

    [Route("update")]
    [HttpPost]
    [Authorize]
    [ValidateUser]
    public async Task<IActionResult> UpdateUser([FromBody] ApiUserDto apiUserDto, [FromQuery] string approvailJwtToken, CancellationToken cancellationToken)
    {
        Guid userGuid = HttpContext.GetGuidOrDefaultFromRequest();

        if (JwtIsValid(approvailJwtToken, out _) is false)  
        {
            return BadRequest(new ViApiResponse<string>(approvailJwtToken, false, "Сигнатура токена не подтверждена или токен не для этой операции"));
        }

        var validationResult = await new ApiUserDtoValidator().ValidateAsync(apiUserDto, cancellationToken);
        if (validationResult.IsValid is false)
        {
            return BadRequest(new ViApiResponse<ValidationResult>(validationResult, false, "Ошибка валидации полей пользователя"));
        }

        string hash = BCrypt.Net.BCrypt.EnhancedHashPassword(apiUserDto.Password);
        var apiUser = new ApiUser(userGuid, apiUserDto.Firstname, apiUserDto.Username, apiUserDto.Email, hash);

        await _repo.UpdateApiUser(apiUser, cancellationToken);
        return Ok(new ViApiResponse<string>("OK", false, "Данные обновлены"));

    }

    [Route("deleteme")]
    [HttpGet]
    [Authorize]
    [ValidateUser]
    public async Task<IActionResult> DeleteUser([FromQuery] string approvailJwtToken, CancellationToken cancellationToken)
    {
        Guid userGuid = HttpContext.GetGuidOrDefaultFromRequest();

        if (JwtIsValid(approvailJwtToken, out _) is false)
        {
            return BadRequest(new ViApiResponse<string>(approvailJwtToken, false, "Сигнатура токена не подтверждена или токен не для этой операции"));
        }

        await _repo.DeleteApiUserAsync(userGuid, cancellationToken);
        return Ok(new ViApiResponse<string>("OK", false, "Пользователь и все данные о нем удалены"));
    }
    private bool JwtIsValid(string approvailJwtToken, out ClaimsPrincipal? claims)
    {
        var validationParameters = new TokenValidationParameters()
        {
            ClockSkew = TimeSpan.FromMinutes(5),
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidateIssuerSigningKey = true,
            ValidateTokenReplay = false,
            ValidateLifetime = true,
            IssuerSigningKey = _jwtConf.GetSymmetricSecurityKey(),
            ValidAudience = _jwtConf.Audience,
            ValidIssuer = _jwtConf.Issuer
        };

        try
        {
            claims = new JwtSecurityTokenHandler().ValidateToken(approvailJwtToken, validationParameters, out SecurityToken validatedToken);

            //Проверка токена на наличие t емаил, чтобы нельзя было поставить другой токен, выпущенный этим сервером.
            bool containEmail = claims.Claims.Any(c => c.Type == ClaimTypes.Email);
            return containEmail;
        }
        catch (Exception)
        {
            claims = null;
            return false;
        }
    }
}
