using FluentValidation;
using ViApi.Types.Configuration;

namespace ViApi.Validation.Fluent;

public class BotValidation : AbstractValidator<BotConfiguration>
{
    public BotValidation()
    {
        RuleFor(b => b.BotToken).NotEmpty();
        RuleFor(b => b.WebhookSecretToken).NotEmpty();
        RuleFor(b => b.WebhookUrl).NotEmpty();
    }
}

public class JwtValidation : AbstractValidator<JwtConfiguration>
{
    public JwtValidation()
    {
        RuleFor(j=>j.Audience).NotEmpty();
        RuleFor(j=>j.Issuer).NotEmpty();
        RuleFor(j=>j.JwtKey).NotEmpty();
        RuleFor(j=>j.TokenLifeTime).NotEmpty();
    }
}

public class DbValidation : AbstractValidator<DbConfiguration>
{
    public DbValidation()
    {
        RuleFor(d => d.MongoDbConnString).NotEmpty();
        RuleFor(d => d.MongoDbName).NotEmpty();
        RuleFor(d => d.MySqlConnString).NotEmpty();
    }
}