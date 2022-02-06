using Microsoft.AspNetCore.Mvc;

namespace app.Controllers
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [ApiController]
    [Route("/")]
    public class AppController : ControllerBase
    {
        [HttpGet]
        public ActionResult toSwagger() => Redirect("/swagger");
    }
}