using Microsoft.AspNetCore.Mvc;

namespace emedl_chase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentPostingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
