using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;
using ViApi.Services.Telegram.UpdateHandlers;
using ViApi.Validation;

namespace ViApi.Contollers;

[Route("/bot")]
[ApiController]
public class BotController : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post([FromBody] Update update, [FromServices] UpdateHandler handleUpdateService, CancellationToken cancellationToken)
    {
        await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
        return Ok();
    }
}
