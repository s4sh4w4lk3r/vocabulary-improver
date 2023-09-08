using FluentValidation;
using ViApi.Types.Common.Users;

namespace ViApi.Validation.Fluent.UserValidation;

public class UserBaseValidator : AbstractValidator<UserBase>
{
    public UserBaseValidator()
    {
        RuleFor(u => u.Firstname).NotEmpty();
        RuleFor(u => u.Guid).NotEmpty();
    }
}
