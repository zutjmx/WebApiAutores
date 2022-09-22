using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Common;
using WebApiAutores.Entidades;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet] // GET: api/<AutoresController>
        [HttpGet("listado")] // GET: api/<AutoresController>/listado
        [HttpGet("/listado")] // GET: listado
        public async Task<ActionResult<List<Autor>>> Get()
        {
            return await context.Autores.Include(x => x.Libros).ToListAsync();
        }

        // GET api/<AutoresController>/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Autor>> Get(int id)
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
        public async Task<ActionResult<Autor>> Get(string nombre)
        {
            Autor autor = await context.Autores.FirstOrDefaultAsync(x => x.Nombre.ToLower().Contains(nombre.ToLower()));
            if (autor == null)
            {
                return NotFound($"No existe el autor con Nombre:{nombre} en la base de datos");
            }
            return autor;
        }

        // GET api/<AutoresController>/primerautor
        [HttpGet("primerautor")]
        public async Task<ActionResult<Autor>> GetPrimerAutor()
        {
            return await context.Autores.FirstOrDefaultAsync();
        }

        // POST api/<AutoresController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor)
        {
            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        // PUT api/<AutoresController>/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] Autor autor)
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
        public async Task<ActionResult> Delete(int id)
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
