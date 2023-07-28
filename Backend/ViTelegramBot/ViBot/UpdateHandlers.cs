using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using ViTelegramBot.ApiRequesting;
using ViTelegramBot.Entities;

namespace ViTelegramBot.ViBot;

public static class UpdateHandlers
{
    public static async Task OnStartAsync(ServiceProvider serviceProvider, ViApiClient viApi, Update update)
    {
        ChatId chatId = new(update.Message!.Chat.Id);
        ITelegramBotClient botClient = serviceProvider.GetRequiredService<ITelegramBotClient>();

        var result = await viApi.SignUpUserAsync(chatId.Identifier.GetValueOrDefault(), update.Message!.Chat.FirstName!);
        if (result.ResultCode is ViResultTypes.Created)
        {
            await botClient.SendTextMessageAsync(chatId, "Вы зарегистировались!");
        }

        if (result.ResultCode == ViResultTypes.Founded)
        {
            await botClient.SendTextMessageAsync(chatId, "Снова здраствуйте!");
        }
    }
}
