using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers.Version_1_0
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("[controller]")]
    public class DebugController : ControllerBase
    {
        public DebugController()
        {
        }
        // GET: api/<DebugController>
        [HttpGet]
        public string Get()
        {
            return "api1";
        }

        // GET api/<DebugController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DebugController>
        [HttpPost]
        public string Post([FromBody] string value)
        {
            return value + "success";
            //return asd + "wwwww";
        }

        // PUT api/<DebugController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DebugController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
