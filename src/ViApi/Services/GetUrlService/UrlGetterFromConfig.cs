namespace ViApi.Services.GetUrlService;

public class UrlGetterFromConfig : IUrlGetter
{
    private string UrlFromConfig { get; set; }
    public UrlGetterFromConfig(string urlFromConfig)
    {
        urlFromConfig.Throw("Передан плохой URL в конструктор URL геттера.").IfNullOrWhiteSpace(s => s);
        UrlFromConfig = urlFromConfig;
    }
    public string GetUrl()
    {
        return UrlFromConfig;
    }
}
