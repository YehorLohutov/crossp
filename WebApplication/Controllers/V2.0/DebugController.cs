using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Controllers.Version_2_0
{
    [ApiVersion("2.0")]
    [ApiController]
    [Route("[controller]")]
    public class DebugController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "api2";
        }
    }
}
