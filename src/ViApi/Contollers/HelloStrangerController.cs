using Microsoft.AspNetCore.Mvc;

namespace ViApi.Contollers
{
    [Route("/")]
    [ApiController]
    public class HelloStrangerController : ControllerBase
    {
        public async Task SayHello()
        {
            Response.ContentType = "text/html;charset=utf-8";
            await Response.WriteAsync("<h1>Тебе здесь не рады.</h1>");
        }
    }
}
