namespace ViApi.Types.Configuration;

public class BotConfiguration
{
    private string botToken = null!;
    private string webhookSecretToken = null!;
    private string? webhookUrl = null!;

    public required string BotToken { get => botToken; init => botToken = value.Throw().IfNullOrWhiteSpace(s => s).Value; }
    public required string WebhookSecretToken { get => webhookSecretToken; init => webhookSecretToken = value.Throw().IfNullOrWhiteSpace(s => s).Value; }
    public required string? WebhookUrl { get => webhookUrl; init => webhookUrl = value; }
}
