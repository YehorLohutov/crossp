using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication.Controllers
{
    [ApiVersion("1.0")]
    [ApiController]
    [Route("V{version:apiVersion}/[controller]")]
    public class DebugController : ControllerBase
    {
        private ApplicationDBContext ApplicationContext;
        public DebugController(ApplicationDBContext applicationContext)
        {
            ApplicationContext = applicationContext;
        }
        // GET: api/<DebugController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
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
