using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using ViApi.Services.Repository;
using ViApi.Types.API;

namespace ViApi.Contollers;

[Route("/api/register")]
[ApiController]
public partial class RegistrationController : ControllerBase
{
    [HttpPost]
    public async Task<ViApiResponse<ApiUserDto>> Register(ApiUserDto apiUserDto, [FromServices] IRepository repo)
    {
        var regexpassword = PasswordRegex();
        var regexemail = Email();
        if (regexpassword.IsMatch(apiUserDto.Password) is false)
        {
            return new ViApiResponse<ApiUserDto>(apiUserDto, false, "Слабый пароль");
        }

        if (regexemail.IsMatch(apiUserDto.Email) is false)
        {
            return new ViApiResponse<ApiUserDto>(apiUserDto, false, "Email имеет неверный формат");
        }

        var apiUser = new Types.Common.Users.ApiUser(Guid.NewGuid(), apiUserDto.Firstname, apiUserDto.Username, apiUserDto.Email, apiUserDto.Password);
        bool dbInsertOk = await repo.InsertUserAsync(apiUser);

        if (dbInsertOk)
        {
            return new ViApiResponse<ApiUserDto>(apiUserDto, true, "Регистрация прошла успешно");
        }
        else
        {
            return new ViApiResponse<ApiUserDto>(apiUserDto, false, "Регистрация не прошла, с таким email и/или username пользователь уже заргистрирован");
        }
    }

    public record class ApiUserDto(string Username, string Email, string Firstname, string Password);


    [GeneratedRegex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$")]
    private static partial Regex PasswordRegex();

    [GeneratedRegex("^\\S+@\\S+\\.\\S+$")]
    private static partial Regex Email();
}