using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Common;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;
using WebApiAutores.Filtros;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController(
            ApplicationDbContext context,
            IMapper mapper, 
            IConfiguration configuration
        )
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        // Para probar proveedores de configuración ini
        //[HttpGet("version")]
        //public ActionResult<string> ObtenerVersion()
        //{
        //    return configuration["version"];
        //}

        //[HttpGet("cadenaConexion")]
        //public ActionResult<string> ObtenerCadenaConexion()
        //{
        //    return configuration["connectionStrings:defaultConnection"];
        //}

        //[HttpGet("versionBD")]
        //public ActionResult<string> ObtenerVersionBD()
        //{
        //    return configuration["versionBD"];
        //}

        //[HttpGet("seedToken")]
        //public ActionResult<string> ObtenerSeedToken()
        //{
        //    return configuration["seedToken"];
        //}
        // Para probar proveedores de configuración fin

        [HttpGet] // GET: api/<AutoresController>
        [AllowAnonymous]
        public async Task<ActionResult<List<AutorDto>>> Get()
        {
            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDto>>(autores);
        }

        // GET api/<AutoresController>/5
        [HttpGet("{id:int}",Name ="obtenerAutor")]
        public async Task<ActionResult<AutorDtoConLibros>> Get([FromRoute] int id)
        {
            var autor = await context.Autores
                .Include(autorDB => autorDB.AutoresLibros)
                .ThenInclude(autorLibroDB => autorLibroDB.Libro)
                .FirstOrDefaultAsync(autorBD => autorBD.Id == id);

            if (autor == null)
            {
                return NotFound($"No existe el autor con Id:{id} en la base de datos");
            }

            return mapper.Map<AutorDtoConLibros>(autor);
        }

        // GET api/<AutoresController>/nombre
        [HttpGet("{nombre}")]
        public async Task<ActionResult<List<AutorDto>>> Get([FromRoute] string nombre)
        {
            var autores = await context.Autores.Where(autorBD => autorBD.Nombre.ToLower().Contains(nombre.ToLower())).ToListAsync();
            return mapper.Map<List<AutorDto>>(autores);
        }

        // POST api/<AutoresController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorCreacionDto autorCreacionDto)
        {
            var existeAutorConMismoNombre = await context.Autores.AnyAsync(x => x.Nombre == autorCreacionDto.Nombre);
            if(existeAutorConMismoNombre)
            {
                return BadRequest($"Ya existe un autor con nombre: [{autorCreacionDto.Nombre}]");
            }
            var autor = mapper.Map<Autor>(autorCreacionDto);
            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDto = mapper.Map<AutorDto>(autor);

            return CreatedAtRoute("obtenerAutor",new {id = autor.Id}, autorDto);
        }

        // PUT api/<AutoresController>/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromRoute] int id, [FromBody] AutorCreacionDto autorCreacionDto)
        {
            
            bool existeAutor = await context.Autores.AnyAsync(autor => autor.Id == id);
            if (!existeAutor)
            {
                return NotFound($"No existe el autor con Id:{id} en la base de datos");
            }

            var autor = mapper.Map<Autor>(autorCreacionDto);
            autor.Id = id;

            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();
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
