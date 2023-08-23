using NgrokApi;

namespace ViApi.Services.GetUrlService
{
    public class UrlGetterFromNgrok : IUrlGetter
    {
        private Ngrok Ngrok { get; set; }
        public UrlGetterFromNgrok(string ngrokApiToken)
        {
            ngrokApiToken.Throw("NgrokApiToken не введен.").IfWhiteSpace();
            Ngrok = new Ngrok(ngrokApiToken);
        }

        public string GetUrl()
        {
            return GetFirstTunnelUrlAsync().Result;
        }
        private async Task<string> GetFirstTunnelUrlAsync(CancellationToken cancellationToken = default)
        {
            var tunnel = await Ngrok.Tunnels.List().FirstOrDefaultAsync(cancellationToken);
            string url = tunnel?.PublicUrl!;
            url.Throw(_ => new InvalidOperationException("Ngrok URL не получен.")).IfNullOrEmpty(u => u);
            return url;
        }
    }
}
