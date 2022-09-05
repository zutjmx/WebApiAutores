using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Entidades;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;

        public LibrosController(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        // GET: api/<LibrosController>
        [HttpGet]
        public async Task<ActionResult<List<Libro>>> Get()
        {
            return await this.dbContext.Libros.ToListAsync();
        }

        // GET api/<LibrosController>/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Libro>> Get(int id)
        {
            return await this.dbContext.Libros.Include(x => x.Autor).FirstOrDefaultAsync(libro => libro.Id == id);
        }

        // POST api/<LibrosController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Libro libro)
        {
            bool existeAutor = await dbContext.Autores.AnyAsync(autor => autor.Id == libro.AutorId);
            if (!existeAutor)
            {
                return BadRequest($"No existe un autor con id {libro.AutorId}");
            }
            this.dbContext.Add(libro);
            await this.dbContext.SaveChangesAsync();
            return Ok();
        }

        // PUT api/<LibrosController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<LibrosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
