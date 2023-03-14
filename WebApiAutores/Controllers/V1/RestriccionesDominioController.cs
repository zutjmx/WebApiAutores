using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Migrations;

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

        //// GET: api/<RestriccionesDominioController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<RestriccionesDominioController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<RestriccionesDominioController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] CrearRestriccionesDominioDTO crearRestriccionesDominioDTO)
        {
            var llaveDB = await context.LlavesAPI
                .FirstOrDefaultAsync(x => x.Id == crearRestriccionesDominioDTO.LlaveId);

            if(llaveDB == null)
            {
                return NotFound();
            }

            var usuarioId = ObtenerUsuario();

            if(llaveDB.UsuarioId != usuarioId) { return Forbid(); }

            var restriccionesDominio = new RestriccionDominio()
            {
                LlaveId = crearRestriccionesDominioDTO.LlaveId,
                Dominio = crearRestriccionesDominioDTO.Dominio
            };

            context.Add(restriccionesDominio);
            await context.SaveChangesAsync();
            return NoContent();

        }

        // PUT api/<RestriccionesDominioController>/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] ActualizarRestriccionDominioDTO actualizarRestriccionDominioDTO)
        {
            var restriccionDB = await context.RestriccionesDominio
                .Include(x => x.Llave)
                .FirstOrDefaultAsync(x => x.Id == id);

            if(restriccionDB == null) { return NotFound(); }

            var usuarioId = ObtenerUsuario();

            if (restriccionDB.Llave.UsuarioId != usuarioId) { return Forbid(); }

            restriccionDB.Dominio = actualizarRestriccionDominioDTO.Dominio;
            await context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<RestriccionesDominioController>/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var restriccionDB = await context.RestriccionesDominio
                .Include(x => x.Llave)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (restriccionDB == null) { return NotFound(); }

            var usuarioId = ObtenerUsuario();

            if (restriccionDB.Llave.UsuarioId != usuarioId) { return Forbid(); }

            context.Remove(restriccionDB);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
