using emedl_chase.Helper;
using emedl_chase.Option;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace emedl_chase.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class EPICController : ControllerBase
    {
        private readonly ApplicationConfig _config;

        public EPICController (IOptions<ApplicationConfig> appconfig)
        {
            _config = appconfig.Value;
        }



        [HttpGet("epicjwtassertion")]
        public async Task<IActionResult> Index()
        {
            #region  jwtassertionget
            var clinetid = _config.EpicNonprodClinetid;
            var tokenurl=_config.EpicSandboxOauthUrl;
            var privatepath =_config.EpicPrivatekeyPath;
            #endregion
            var jwtassertion = EPICTokenHelper.GenerateEcwJwt(privatepath, clinetid, tokenurl);

            var token = await EPICTokenHelper.GetAccessTokenAsync(privatepath,clinetid,tokenurl);

            return Ok(jwtassertion);
        }
    }
}
