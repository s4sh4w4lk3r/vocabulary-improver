using FluentValidation;
using ViApi.Types.Common;

namespace ViApi.Validation.Fluent
{
    public class DictionaryValidator : AbstractValidator<Dictionary>
    {
        public DictionaryValidator()
        {
            RuleFor(d=> d.Guid).NotEmpty();
            RuleFor(d=> d.Name).NotEmpty();
            RuleFor(d=> d.UserGuid).NotEmpty();
            RuleFor(d=> d.Guid).NotEmpty();
        }
    }
}
