using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers
{
    [Route("api/libros/{libroId:int}/[controller]")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ComentariosController(ApplicationDbContext context, IMapper mapper) 
        {
            this.context = context;
            this.mapper = mapper;
        }

        // GET: api/<ComentariosController>
        [HttpGet]
        public async Task<ActionResult<List<ComentarioDto>>> Get(int libroId)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existeLibro)
            {
                return NotFound($"No existe el libro con ID: {libroId}");
            }

            var comentarios = await context.Comentarios
                .Where(comentarioBD => comentarioBD.LibroId == libroId)
                .ToListAsync();

            return mapper.Map<List<ComentarioDto>>(comentarios);
        }

        //// GET api/<ComentariosController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<ComentariosController>
        [HttpPost]
        public async Task<ActionResult> Post(int libroId, [FromBody] ComentarioCreacionDto comentarioCreacionDto)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existeLibro)
            {
                return NotFound($"No existe el libro con ID: {libroId}");
            }
            var comentario = mapper.Map<Comentario>(comentarioCreacionDto);
            comentario.LibroId = libroId;
            context.Add(comentario);
            await context.SaveChangesAsync();
            return Ok(comentario);
            //return Ok();
        }

        // PUT api/<ComentariosController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ComentariosController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
