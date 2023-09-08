using FluentValidation;
using ViApi.Types.Telegram;

namespace ViApi.Validation.Fluent
{
    public class TelegramSessionValidator : AbstractValidator<TelegramSession>
    {
        public TelegramSessionValidator()
        {
            RuleFor(ts => ts.TelegramId).NotEmpty();
            RuleFor(ts => ts.UserGuid).NotEmpty();
        }
    }
}
