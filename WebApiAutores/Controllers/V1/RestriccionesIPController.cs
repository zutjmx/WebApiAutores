using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RestriccionesIPController : CustomBaseController
    {
        private readonly ApplicationDbContext context;

        public RestriccionesIPController(ApplicationDbContext context)
        {
            this.context = context;
        }

        //// GET: api/<RestriccionesIPController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<RestriccionesIPController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<RestriccionesIPController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CrearRestriccionIPDTO crearRestriccionIPDTO)
        {
            var llaveDB = await context.LlavesAPI
                .FirstOrDefaultAsync(x => x.Id == crearRestriccionIPDTO.LlaveId);
            
            if (llaveDB == null)
            {
                return NotFound();
            }

            var usuarioId = ObtenerUsuario();

            if(usuarioId != llaveDB.UsuarioId) { return Forbid(); }

            var restriccionIP = new RestriccionIP()
            {
                LlaveId = llaveDB.Id,
                IP = crearRestriccionIPDTO.IP
            };

            context.Add(restriccionIP);
            await context.SaveChangesAsync();

            return NoContent();
        }

        // PUT api/<RestriccionesIPController>/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] ActualizarRestriccionIPDTO actualizarRestriccion)
        {
            var restriccionDB = await context.RestriccionesIP.Include(x => x.Llave)
            .FirstOrDefaultAsync(x => x.Id == id);

            if (restriccionDB == null)
            {
                return NotFound();
            }

            var usuarioId = ObtenerUsuario();

            if (restriccionDB.Llave.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            restriccionDB.IP = actualizarRestriccion.IP;
            await context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE api/<RestriccionesIPController>/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restriccionDB = await context.RestriccionesIP.Include(x => x.Llave).FirstOrDefaultAsync(x => x.Id == id);

            if (restriccionDB == null)
            {
                return NotFound();
            }

            var usuarioId = ObtenerUsuario();

            if (usuarioId != restriccionDB.Llave.UsuarioId)
            {
                return Forbid();
            }

            context.Remove(restriccionDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
