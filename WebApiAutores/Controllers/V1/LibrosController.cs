using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers.V1
{
    [Route("api/v1/[controller]")]
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
        [HttpGet(Name = "obtenerLibros")]
        public async Task<ActionResult<List<LibroDto>>> Get()
        {
            var libros = await dbContext.Libros.ToListAsync();
            return mapper.Map<List<LibroDto>>(libros);
        }

        // GET api/<LibrosController>/5
        [HttpGet("{id:int}", Name = "obtenerLibro")]
        public async Task<ActionResult<LibroDtoConAutores>> Get(int id)
        {
            var libro = await dbContext.Libros
                .Include(libroDB => libroDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Autor)
                .Include(libroBD => libroBD.Comentarios)
                .FirstOrDefaultAsync(libro => libro.Id == id);

            if (libro == null)
            {
                return NotFound($"No existe el ibro con ID:{id}");
            }

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x => x.Orden).ToList();

            return mapper.Map<LibroDtoConAutores>(libro);
        }

        // POST api/<LibrosController>
        [HttpPost(Name = "crearLibro")]
        public async Task<ActionResult> Post([FromBody] LibroCreacionDto libroCreacionDto)
        {

            if (libroCreacionDto.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }

            var autoresIds = await dbContext.Autores
                .Where(autorDB => libroCreacionDto.AutoresIds.Contains(autorDB.Id))
                .Select(x => x.Id)
                .ToListAsync();

            if (libroCreacionDto.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroCreacionDto);

            AplicarOrdenLibros(libro);

            dbContext.Add(libro);
            await dbContext.SaveChangesAsync();

            var libroDto = mapper.Map<LibroDto>(libro);

            return CreatedAtRoute("obtenerLibro", new { id = libro.Id }, libroDto);
        }

        // PUT api/<LibrosController>/5
        [HttpPut("{id:int}", Name = "actualizarLibro")]
        public async Task<ActionResult> Put(int id, [FromBody] LibroCreacionDto libroCreacionDto)
        {
            var libroDB = await dbContext.Libros
                .Include(x => x.AutoresLibros)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (libroDB == null)
            {
                return NotFound($"No existe el libro con ID:{id}");
            }

            libroDB = mapper.Map(libroCreacionDto, libroDB);

            AplicarOrdenLibros(libroDB);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "patchLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDto> jsonPatchDocument)
        {
            if (jsonPatchDocument == null)
            {
                return BadRequest();
            }

            var libroBD = await dbContext.Libros.FirstOrDefaultAsync(x => x.Id == id);
            if (libroBD == null)
            {
                return NotFound($"No existe el libro con ID:{id}");
            }

            var libroDto = mapper.Map<LibroPatchDto>(libroBD);
            jsonPatchDocument.ApplyTo(libroDto, ModelState);

            var esValido = TryValidateModel(libroDto);
            if (!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDto, libroBD);

            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<LibrosController>/5
        [HttpDelete("{id:int}", Name = "borrarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            var existeLibro = await dbContext.Libros.AnyAsync(x => x.Id == id);

            if (!existeLibro)
            {
                return NotFound($"No existe el libro con ID:{id}");
            }

            dbContext.Remove(new Libro { Id = id });
            await dbContext.SaveChangesAsync();

            return NoContent();
        }

        private static void AplicarOrdenLibros(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }

    }
}
