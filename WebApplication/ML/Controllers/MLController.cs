using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ML.Controllers
{
    [ApiVersion("1.0")]
    [Authorize]
    [ApiController]
    [Route("ml")]
    public class MLController : ControllerBase
    {
        [HttpGet]
        [ApiConventionMethod(typeof(DefaultApiConventions), nameof(DefaultApiConventions.Get))]
        [AllowAnonymous]
        [Route("version")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        public ActionResult<string> Version(ApiVersion apiVersion) => Ok($"Active {nameof(MLController)} API ver is {apiVersion}");

    }
}
