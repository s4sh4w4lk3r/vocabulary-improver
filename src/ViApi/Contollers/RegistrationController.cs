using Microsoft.AspNetCore.Mvc;
using ViApi.Services.Repository;
using ViApi.Types.API;

namespace ViApi.Contollers;

[Route("/api/register")]
[ApiController]
public partial class RegistrationController : ControllerBase
{
    [HttpPost]
    public async Task<ViApiResponse<ApiUserDto>> Register(ApiUserDto apiUserDto, [FromServices] IRepository repo, CancellationToken cancellationToken)
    {
#warning добавить фильтр сюды.
        var hash = BCrypt.Net.BCrypt.EnhancedHashPassword(apiUserDto.Password);
        var apiUser = new Types.Common.Users.ApiUser(Guid.NewGuid(), apiUserDto.Firstname, apiUserDto.Username, apiUserDto.Email, hash);

        bool dbInsertOk = await repo.InsertUserAsync(apiUser, cancellationToken);

        if (dbInsertOk)
        {
            return new ViApiResponse<ApiUserDto>(apiUserDto, true, "Регистрация прошла успешно");
        }
        else
        {
            return new ViApiResponse<ApiUserDto>(apiUserDto, false, "Регистрация не прошла, с таким email и/или username пользователь уже заргистрирован");
        }
    }

}