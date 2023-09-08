using FluentValidation;
using ViApi.Types.Common.Users;

namespace ViApi.Validation.Fluent.UserValidation;

public class ApiUserValidator : AbstractValidator<ApiUser>
{
    public ApiUserValidator()
    {
        Include(new UserBaseValidator());
        RuleFor(u => u.Email).Matches("^\\S+@\\S+\\.\\S+$").WithMessage("Неверный Email формат.");

        RuleFor(u => u.Password).Matches("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{12,}$")
            .WithMessage("Пароль должен включать в себя большие и маленькие латинские буквы, цифры, спецсимволы(#?!@$%^&*-), также минимальную длину 12 символов.");

        RuleFor(u => u.Username).NotEmpty();

    }
}
