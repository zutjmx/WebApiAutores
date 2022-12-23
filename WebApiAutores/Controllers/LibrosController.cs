using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        public async Task<ActionResult<LibroDtoConAutores>> Get(int id)
        {
            var libro = await this.dbContext.Libros
                .Include(libroDB => libroDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Autor)
                .Include(libroBD => libroBD.Comentarios)
                .FirstOrDefaultAsync(libro => libro.Id == id);

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDtoConAutores>(libro);
        }

        // POST api/<LibrosController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] LibroCreacionDto libroCreacionDto)
        {

            if(libroCreacionDto.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autoresIds = await dbContext.Autores
                .Where(autorDB => libroCreacionDto.AutoresIds.Contains(autorDB.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if(libroCreacionDto.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroCreacionDto);

            if(libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
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
