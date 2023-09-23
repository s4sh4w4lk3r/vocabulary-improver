using Microsoft.AspNetCore.Mvc;
using ViApi.Services.Repository;
using ViApi.Types.API;
using ViApi.Validation.Fluent.DtoValidators;

namespace ViApi.Contollers.UserControllers;

[Route("/api/register")]
[ApiController]
public partial class RegistrationController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Register(ApiUserDto apiUserDto, [FromServices] IRepository repo, CancellationToken cancellationToken)
    {
        var validationResult = await new ApiUserDtoValidator().ValidateAsync(apiUserDto, cancellationToken);
        if (validationResult.IsValid is false)
        {
            return BadRequest(new ViApiResponse<ApiUserDto>(apiUserDto, false, validationResult.ToString()));
        }

        var hash = BCrypt.Net.BCrypt.EnhancedHashPassword(apiUserDto.Password);
        var apiUser = new Types.Common.Users.ApiUser(Guid.NewGuid(), apiUserDto.Firstname, apiUserDto.Username, apiUserDto.Email, hash);

        bool dbInsertOk = await repo.InsertUserAsync(apiUser, cancellationToken);

        if (dbInsertOk)
        {
            return Ok(new ViApiResponse<ApiUserDto>(apiUserDto, true, "Регистрация прошла успешно"));
        }
        else
        {
            return BadRequest(new ViApiResponse<ApiUserDto>(apiUserDto, false, "Регистрация не прошла, с таким email и/или username пользователь уже зарегистрирован"));
        }
    }

}