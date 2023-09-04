using FluentValidation;
using ViApi.Types.Common;

namespace ViApi.Validation.Fluent;

public class WordValidator : AbstractValidator<Word>
{
    public WordValidator()
    {
        RuleFor(w => w.Guid).NotEmpty();
        RuleFor(w => w.DictionaryGuid).NotEmpty();
        RuleFor(w => w.SourceWord).NotEmpty();
        RuleFor(w => w.TargetWord).NotEmpty();
    }
}
