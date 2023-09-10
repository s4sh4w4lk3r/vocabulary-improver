using System.Security.Claims;

namespace ViApi.Contollers
{
    public static class ControllerExtensions
    {
        public static Guid GetGuidOrDefaultFromRequest(this HttpContext context)
        {
            var claim = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && (c.Value is not null));
            _ = Guid.TryParse(claim?.Value, out Guid guid);
            return guid;
        }
    }
}