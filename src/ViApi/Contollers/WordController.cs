﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ViApi.Services.Repository;
using ViApi.Types.API;
using ViApi.Types.Common;
using ViApi.Validation.Filters;

namespace ViApi.Contollers;

[Route("/api/words")]
[ApiController]
public class WordController : ControllerBase
{
    private readonly IRepository _repo;
    public WordController([FromServices] IRepository repository)
    {
        _repo = repository;
    }

    [Route("add")]
    [HttpPost]
    [Authorize]
    [ValidateUser]
    public async Task<ActionResult>AddWord([FromBody] [Bind("SourceWord", "TargetWord", "DictionaryGuid")] Word word, CancellationToken cancellationToken)
    {
        Guid userGuid = HttpContext.GetGuidOrDefaultFromRequest();

        if (string.IsNullOrWhiteSpace(word.SourceWord) || string.IsNullOrWhiteSpace(word.TargetWord) || word.DictionaryGuid == default)
        {
            return BadRequest(new ViApiResponse<Word>(word, false, "Какие-то поля пустые или имеют дефолтные значения"));
        }

        bool InsertOk = await _repo.InsertWordAsync(userGuid, word.DictionaryGuid, word.SourceWord, word.TargetWord, cancellationToken);
        if (InsertOk is true)
        {
            return Ok(new ViApiResponse<Word>(word, true, "Слово добавлено"));
        }
        else
        {
            return BadRequest(new ViApiResponse<Word>(word, false, "Слово не добавилось, возможно словарь не принадлежит пользователю"));
        }

    }

    [Route("delete")]
    [HttpGet]
    [Authorize]
    [ValidateUser]
    public async Task<ActionResult> DeleteWord(Guid wordGuid, Guid dictGuid, CancellationToken cancellationToken)
    {
        Guid userGuid = HttpContext.GetGuidOrDefaultFromRequest();
        if (wordGuid == default || dictGuid == default)
        {
            return BadRequest(new ViApiResponse<IEnumerable<Guid>>(new Guid[] { wordGuid, dictGuid }, false, "Получен пустой Guid. Первый - wordguid, второй - dictguid"));
        }

        bool isRemoved = await _repo.DeleteWordAsync(userGuid, dictGuid, wordGuid, cancellationToken);
        if (isRemoved is true)
        {
            return Ok(new ViApiResponse<string>("Ok", true, "Слово удалено"));
        }
        else
        {
            return BadRequest(new ViApiResponse<IEnumerable<Guid>>(new Guid[] { wordGuid, dictGuid }, false, "Слово не удалено, возможно нет связи между словом, словарем, пользователем. Первый - wordguid, второй - dictguid"));
        }
    }

    [Route("updaterating")]
    [HttpPost]
    [Authorize]
    [ValidateUser]
    public async Task<ActionResult> UpdateWordRating([FromBody] [Bind("Guid", "DictionaryGuid")]Word word, RatingAction ratingAction, CancellationToken cancellationToken)
    {
        Guid userGuid = HttpContext.GetGuidOrDefaultFromRequest();
        Guid dictGuid = word.DictionaryGuid;
        if (dictGuid == default)
        {
            return BadRequest(new ViApiResponse<Guid>(dictGuid, false, "Получен пустой dictGuid"));
        }
        if (word.Guid == default)
        {
            return BadRequest(new ViApiResponse<Word>(word, false, "Слово имеет пустое Guid поле"));
        }

        int rating = await _repo.UpdateWordRating(userGuid, dictGuid, word.Guid, ratingAction, cancellationToken);
        if (rating != -1)
        {
            return Ok(new ViApiResponse<int>(rating, true, "Рейтинг обновлен"));
        }
        else
        {
            return BadRequest(new ViApiResponse<int>(rating, false, "Слово не найдено."));
        }
    }
    
    [Route("getlist")]
    [HttpGet]
    [Authorize]
    [ValidateUser]
    public async Task<ActionResult> GetWordList(Guid dictGuid, CancellationToken cancellationToken)
    {
        Guid userGuid = HttpContext.GetGuidOrDefaultFromRequest();
        if (dictGuid == default) 
        {
            return BadRequest(new ViApiResponse<Guid>(dictGuid, false, "Получен пустой dictGuid."));
        }

        var words = await _repo.GetWordsAsync(userGuid, dictGuid, cancellationToken);
        if (words is null)
        {
            return BadRequest(new ViApiResponse<Guid>(dictGuid, false, "Словарь не найден."));
        }

        return Ok(new ViApiResponse<List<Word>>(words, true, $"Словарь найден, слов: {words.Count}"));
    }

    [Route("addwordlist")]
    [HttpPost]
    [Authorize]
    [ValidateUser]
    public async Task<ActionResult> AddWordList([FromBody]IEnumerable<WordDto> dtoWords, [FromQuery] Guid dictGuid, CancellationToken cancellationToken)
    {
        Guid userGuid = HttpContext.GetGuidOrDefaultFromRequest();
        if (dictGuid == default)
        {
            return BadRequest(new ViApiResponse<Guid>(dictGuid, false, "Получен пустой dictGuid."));
        }

        if (dtoWords is null || !dtoWords.Any())
        {
            return BadRequest(new ViApiResponse<IEnumerable<WordDto>>(dtoWords, false, "Слова не получены"));
        }

        var words = new List<Word>(dtoWords.Count());

        foreach (var dtoWord in dtoWords)
        {
            if (string.IsNullOrWhiteSpace(dtoWord.SourceWord) || string.IsNullOrWhiteSpace(dtoWord.TargetWord)) 
            {
                return BadRequest(new ViApiResponse<WordDto>(dtoWord, false, "Обнаружено неполное словосочетание")); 
            }
            var newWord = new Word(Guid.NewGuid(), dtoWord.SourceWord, dtoWord.TargetWord, dictGuid);
            words.Add(newWord);
        }
        await _repo.InsertWordListAsync(words, dictGuid, cancellationToken);
        return Ok(new ViApiResponse<List<Word>>(words, true, "Слова добавлены"));
    }
}
