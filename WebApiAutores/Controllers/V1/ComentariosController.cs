using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers.V1
{
    [Route("api/v1/libros/{libroId:int}/[controller]")]
    [ApiController]
    public class ComentariosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(
            ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        // GET: api/<ComentariosController>
        [HttpGet(Name = "obtenerComentarios")]
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

        // GET api/<ComentariosController>/5
        [HttpGet("{id}", Name = "obtenerComentario")]
        public async Task<ActionResult<ComentarioDto>> GetPorId(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id == id);

            if (comentario == null)
            {
                return NotFound($"No existe el comentario con ID:{id}");
            }

            return mapper.Map<ComentarioDto>(comentario);
        }

        // POST api/<ComentariosController>
        [HttpPost(Name = "crearComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, [FromBody] ComentarioCreacionDto comentarioCreacionDto)
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;

            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existeLibro)
            {
                return NotFound($"No existe el libro con ID: {libroId}");
            }
            var comentario = mapper.Map<Comentario>(comentarioCreacionDto);
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;
            context.Add(comentario);
            await context.SaveChangesAsync();

            var comentarioDto = mapper.Map<ComentarioDto>(comentario);

            return CreatedAtRoute("obtenerComentario", new { id = comentario.Id, libroId }, comentarioDto);
        }

        // PUT api/<ComentariosController>/5
        [HttpPut("{id:int}", Name = "actualizarComentario")]
        public async Task<ActionResult> Put(int libroId, int id, [FromBody] ComentarioCreacionDto comentarioCreacionDto)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existeLibro)
            {
                return NotFound($"No existe el libro con ID: {libroId}");
            }

            var existeComentario = await context.Comentarios.AnyAsync(comentarioDB => comentarioDB.Id == id);
            if (!existeComentario)
            {
                return NotFound($"No existe el comentario con ID: {id}");
            }

            var comentario = mapper.Map<Comentario>(comentarioCreacionDto);
            comentario.Id = id;
            comentario.LibroId = libroId;

            context.Update(comentario);
            await context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE api/<ComentariosController>/5
        [HttpDelete("{id}", Name = "borrarComentario")]
        public void Delete(int id)
        {
        }
    }
}
