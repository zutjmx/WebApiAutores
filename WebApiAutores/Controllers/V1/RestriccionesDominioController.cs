using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionesDominioController : CustomBaseController
    {
        private readonly ApplicationDbContext context;

        public RestriccionesDominioController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // GET: api/<RestriccionesDominioController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<RestriccionesDominioController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<RestriccionesDominioController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<RestriccionesDominioController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RestriccionesDominioController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
