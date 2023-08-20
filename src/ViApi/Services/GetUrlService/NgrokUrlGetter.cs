using NgrokApi;
using Throw;

namespace ViApi.Services.GetUrlService
{
    public class NgrokUrlGetter : IUrlGetter
    {
        private Ngrok Ngrok { get; set; }
        public NgrokUrlGetter(string ngrokApiToken)
        {
            ngrokApiToken.Throw("NgrokApiToken не введен.").IfWhiteSpace();
            Ngrok = new Ngrok(ngrokApiToken);
        }

        public string GetUrl()
        {
            return GetFirstTunnelUrlAsync().Result;
        }
        private async Task<string> GetFirstTunnelUrlAsync(int cancellationTokenIntervalSec = 5)
        {
            var cts = new CancellationTokenSource(cancellationTokenIntervalSec);
            var tunnel = await Ngrok.Tunnels.List().FirstOrDefaultAsync(cts.Token);
            string url = tunnel?.PublicUrl!;
            url.Throw(_ => new InvalidOperationException("Ngrok URL не получен.")).IfNullOrEmpty(u => u);
            return url;
        }
    }
}
