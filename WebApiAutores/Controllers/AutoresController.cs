using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Common;
using WebApiAutores.Entidades;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        // GET: api/<AutoresController>
        [HttpGet]
        public ActionResult<List<Autor>> Get()
        {
            Util util = new Util();
            List<Autor> listaAutores = util.GetAutors(20);
            return listaAutores;
        }

        // GET api/<AutoresController>/5
        [HttpGet("{id}")]
        public ActionResult<Autor> Get(int id)
        {
            Util util = new Util();
            Autor autor = util.GetAutor(id);
            return autor;
        }

        // POST api/<AutoresController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AutoresController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AutoresController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
