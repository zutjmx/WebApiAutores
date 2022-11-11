using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibrosController : ControllerBase
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;

        public LibrosController(ApplicationDbContext dbContext, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
        }

        // GET: api/<LibrosController>
        [HttpGet]
        public async Task<ActionResult<List<LibroDto>>> Get()
        {
            var libros = await this.dbContext.Libros.ToListAsync();
            return mapper.Map<List<LibroDto>>(libros);
        }

        // GET api/<LibrosController>/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LibroDto>> Get(int id)
        {
            var libro = await this.dbContext.Libros.FirstOrDefaultAsync(libro => libro.Id == id);
            return mapper.Map<LibroDto>(libro);
        }

        // POST api/<LibrosController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] LibroCreacionDto libroCreacionDto)
        {
            //bool existeAutor = await dbContext.Autores.AnyAsync(autor => autor.Id == libro.AutorId);
            //if (!existeAutor)
            //{
            //    return BadRequest($"No existe un autor con id {libro.AutorId}");
            //}
            var libro = mapper.Map<Libro>(libroCreacionDto);
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
