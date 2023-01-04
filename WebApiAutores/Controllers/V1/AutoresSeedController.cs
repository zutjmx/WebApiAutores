using Microsoft.AspNetCore.Mvc;
using WebApiAutores.Common;
using WebApiAutores.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AutoresSeedController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AutoresSeedController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // POST api/<AutoresSeedController>
        [HttpPost]
        public RespuestaSeedAutor Post([FromBody] int Cuantos)
        {
            Util util = new Util(context);
            RespuestaSeedAutor respuesta = new RespuestaSeedAutor();
            bool respuestaSeed = util.SetSeed(Cuantos);
            if (respuestaSeed)
            {
                respuesta.Codigo = 1;
                respuesta.Mensaje = "Ok";
            }
            else
            {
                respuesta.Codigo = 0;
                respuesta.Mensaje = "Ocurrió un error al ejecutar el método Util.SetSeed";
            }

            return respuesta;
        }

        // DELETE api/<AutoresSeedController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
