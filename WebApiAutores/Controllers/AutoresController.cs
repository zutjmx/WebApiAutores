using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.Common;
using WebApiAutores.Entidades;
using WebApiAutores.Servicios;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IServicio servicio;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScoped servicioScoped;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ILogger<AutoresController> logger;

        public AutoresController(
            ApplicationDbContext context, 
            IServicio servicio,
            ServicioTransient servicioTransient,
            ServicioScoped servicioScoped,
            ServicioSingleton servicioSingleton,
            ILogger<AutoresController> logger
        )
        {
            this.context = context;
            this.servicio = servicio;
            this.servicioTransient = servicioTransient;
            this.servicioScoped = servicioScoped;
            this.servicioSingleton = servicioSingleton;
            this.logger = logger;
        }

        [HttpGet("GUID")]
        public ActionResult GetGuids()
        {
            return Ok(new {
                        AutoresController_Transient = servicioTransient.Guid,
                        ServicioA_Transient = servicio.GetTransient(),
                        AutoresController_Scoped = servicioScoped.Guid,
                        ServicioA_Scoped = servicio.GetScoped(),
                        AutoresController_Singleton = servicioSingleton.Guid,
                        ServicioA_Sigleton = servicio.GetSingleton()
                    });
        }

        [HttpGet] // GET: api/<AutoresController>
        [HttpGet("listado")] // GET: api/<AutoresController>/listado
        [HttpGet("/listado")] // GET: listado
        public async Task<ActionResult<List<Autor>>> Get()
        {
            this.logger.LogInformation("Obteniendo listado de autores");
            servicio.RealizarTarea();
            return await context.Autores.Include(x => x.Libros).ToListAsync();
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

        // GET api/<AutoresController>/primerautor?nombre=Jesús&apellidos=Zúñiga Trejo
        [HttpGet("primerautor")]
        public async Task<ActionResult<Autor>> GetPrimerAutor(
            [FromHeader] int ValorHeader, 
            [FromQuery] string Nombre,
            [FromQuery] string Apellidos
        )
        {
            return await context.Autores.FirstOrDefaultAsync();
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
