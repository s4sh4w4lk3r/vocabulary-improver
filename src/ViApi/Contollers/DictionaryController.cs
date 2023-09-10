using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViApi.Services.Repository;
using ViApi.Types.API;
using ViApi.Types.Common;
using ViApi.Validation.Filters;
using ViApi.Validation.Fluent;

namespace ViApi.Contollers;

[Route("/api/dict")]
[ApiController]
public class DictionaryController : ControllerBase
{
    private readonly IRepository _repo;
    public DictionaryController([FromServices] IRepository repository)
    {
        _repo = repository;
    }

    [Route("getlist")]
    [HttpGet]
    [Authorize]
    [ValidateUser]
    public async Task<IActionResult> GetDictionaryList(CancellationToken cancellationToken)
    {
        Guid userGuid = HttpContext.GetGuidOrDefaultFromRequest();
        var list = await _repo.GetDicionariesList(userGuid, cancellationToken);
        return Ok(new ViApiResponse<List<Dictionary>>(list, true, $"Вот ваши словари, количество: {list.Count}"));
    }

    [Route("rename")]
    [HttpGet]
    [Authorize]
    [ValidateUser]
    public async Task<IActionResult> RenameDictionary(Guid dictGuid, string newName, CancellationToken cancellationToken)
    {
        Guid userGuid = HttpContext.GetGuidOrDefaultFromRequest();

        if (dictGuid == Guid.Empty)
        {
            return BadRequest(new ViApiResponse<Guid>(dictGuid, false, "Получен пустой dictGuid"));
        }

        if (string.IsNullOrWhiteSpace(newName))
        {
            return BadRequest(new ViApiResponse<string>(newName, false, "Новое имя пустое"));
        }

        bool renameOk = await _repo.RenameDictionaryAsync(userGuid, dictGuid, newName, cancellationToken);

        if (renameOk)
        {
            return Ok(new ViApiResponse<Guid>(dictGuid, true, "Словарь переименован"));
        }
        else
        {
            return BadRequest(new ViApiResponse<Guid>(dictGuid, false, "Словарь не переименован, возможно словаря нет"));
        }
    }

    [Route("delete")]
    [HttpGet]
    [Authorize]
    [ValidateUser]
    public async Task<IActionResult> DeleteDictionary(Guid dictGuid, CancellationToken cancellationToken)
    {
        Guid userGuid = HttpContext.GetGuidOrDefaultFromRequest();

        if (dictGuid == Guid.Empty)
        {
            return BadRequest(new ViApiResponse<Guid>(dictGuid, false, "Получен пустой dictGuid"));
        }

        bool deleteOk = await _repo.DeleteDictionaryAsync(userGuid, dictGuid, cancellationToken);

        if (deleteOk)
        {
            return Ok(new ViApiResponse<Guid>(dictGuid, true, "Словарь удален"));
        }
        else
        {
            return BadRequest(new ViApiResponse<Guid>(dictGuid, false, "Словарь не удален, возможно словаря нет"));
        }
    }

    [Route("add")]
    [HttpGet]
    [Authorize]
    [ValidateUser]
    public async Task<IActionResult> AddDictionary(string newName, CancellationToken cancellationToken)
    {
        Guid userGuid = HttpContext.GetGuidOrDefaultFromRequest();


        if (string.IsNullOrWhiteSpace(newName))
        {
            return BadRequest(new ViApiResponse<string>(newName, false, "Имя пустое"));
        }

        var newDict = new Dictionary(Guid.NewGuid(), newName, userGuid);
        var dictValidateResult = await new DictionaryValidator().ValidateAsync(newDict, cancellationToken);

        if (dictValidateResult.IsValid is false)
        {
            return BadRequest(new ViApiResponse<Dictionary>(newDict, false, $"Словарь не добавлен. {dictValidateResult}")) ;
        }

        bool addOk = await _repo.InsertDictionaryAsync(newDict, cancellationToken);

        if (addOk)
        {
            return Ok(new ViApiResponse<Dictionary>(newDict, true, "Словарь добавлен"));
        }
        else
        {
            return BadRequest(new ViApiResponse<Dictionary>(newDict, false, "Словарь не добавлен, возможно пользователя нет в базе"));
        }
    }
}
