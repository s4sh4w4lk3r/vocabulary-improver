using FluentValidation;
using ViApi.Types.Common.Users;

namespace ViApi.Validation.Fluent.UserValidation;

public class TelegramUserValidator : AbstractValidator<TelegramUser>
{
    public TelegramUserValidator()
    {
        Include(new UserBaseValidator());
        RuleFor(u=>u.TelegramId).NotEmpty();
    }
}
