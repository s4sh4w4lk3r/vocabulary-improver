using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace ViApi.Contollers
{
    public static class ControllerExtensions
    {
        public static Guid GetGuidOrDefaultFromRequest(this ControllerBase controller)
        {
            var claim = controller.HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier && (c.Value is not null));
            _ = Guid.TryParse(claim?.Value, out Guid guid);
            return guid;
        }
    }
}