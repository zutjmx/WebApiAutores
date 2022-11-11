using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Common;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        

        public AutoresController(
            ApplicationDbContext context
        )
        {
            this.context = context;
        }

        [HttpGet] // GET: api/<AutoresController>
        public async Task<ActionResult<List<Autor>>> Get()
        {
            return await context.Autores.ToListAsync();
        }

        // GET api/<AutoresController>/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Autor>> Get([FromRoute] int id)
        {
            //Autor autor = await context.Autores.FindAsync(id);
            Autor autor = await context.Autores.FirstOrDefaultAsync(x => x.Id == id);
            if (autor == null)
            {
                return NotFound($"No existe el autor con Id:{id} en la base de datos");
            }
            return autor;
        }

        // GET api/<AutoresController>/nombre
        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> Get([FromRoute] string nombre)
        {
            Autor autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.ToLower().Contains(nombre.ToLower()));
            if (autor == null)
            {
                return NotFound($"No existe el autor con Nombre:{nombre} en la base de datos");
            }
            return autor;
        }

        // POST api/<AutoresController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor)
        {
            var existeAutorConMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);
            if(existeAutorConMismoNombre)
            {
                return BadRequest($"Ya existe un autor con nombre: [{autor.Nombre}]");
            }
            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        // PUT api/<AutoresController>/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody] Autor autor)
        {
            if(autor.Id != id)
            {
                return BadRequest("El Id enviado no coincide con el Id del autor");
            }

            bool existeAutor = await context.Autores.AnyAsync(autor => autor.Id == id);
            if (!existeAutor)
            {
                return NotFound($"No existe el autor con Id:{id} en la base de datos");
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/<AutoresController>/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete([FromRoute] int id)
        {
            bool existeAutor = await context.Autores.AnyAsync(autor => autor.Id == id);
            if(!existeAutor)
            {
                return NotFound($"No existe el autor con Id:{id} en la base de datos");
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
