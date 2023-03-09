using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;
using WebApiAutores.Servicios;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LlavesAPIController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly ServicioLlaves servicioLlaves;

        public LlavesAPIController(ApplicationDbContext context,
            IMapper mapper,
            ServicioLlaves servicioLlaves)
        {
            this.context = context;
            this.mapper = mapper;
            this.servicioLlaves = servicioLlaves;
        }

        // GET: api/<LlavesAPIController>
        [HttpGet]
        public async Task<List<LlaveDTO>> MisLlaves()
        {
            var usuarioId = ObtenerUsuario();
            var llaves = await context.LlavesAPI.Where(x => x.UsuarioId == usuarioId).ToListAsync();
            return mapper.Map<List<LlaveDTO>>(llaves);
        }

        //// GET api/<LlavesAPIController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        // POST api/<LlavesAPIController>
        [HttpPost]
        public async Task<ActionResult> CrearLlave([FromBody] CrearLlaveDTO crearLlaveDTO)
        {
            var usuarioId = ObtenerUsuario();
            
            if(crearLlaveDTO.TipoLlave == Entidades.TipoLlave.Gratuita)
            {
                var yaTieneLlaveGratuita = await context
                    .LlavesAPI
                    .AnyAsync(x => x.UsuarioId == usuarioId && x.TipoLlave == Entidades.TipoLlave.Gratuita);

                if(yaTieneLlaveGratuita)
                {
                    return BadRequest($"El usuario con ID={usuarioId} ya tiene una llave gratuita");
                }
            }

            await servicioLlaves.CrearLlave(usuarioId, crearLlaveDTO.TipoLlave);
            
            return NoContent();
        }

        // PUT api/<LlavesAPIController>/5
        [HttpPut]
        public async Task<ActionResult> ActualizarLlave([FromBody] ActualizarLlaveDTO actualizarLlaveDTO)
        {
            var usuarioId = ObtenerUsuario();
            var llaveDB = await context.LlavesAPI.FirstOrDefaultAsync(x => x.Id == actualizarLlaveDTO.LlaveId);
            
            if(llaveDB == null)
            {
                return NotFound();
            }

            if(usuarioId != llaveDB.UsuarioId)
            {
                return Forbid();
            }

            if(actualizarLlaveDTO.ActualizarLlave)
            {
                llaveDB.Llave = servicioLlaves.GenerarLlave();
            }

            llaveDB.Activa = actualizarLlaveDTO.Activa;

            await context.SaveChangesAsync();

            return NoContent();
        }

        //// DELETE api/<LlavesAPIController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
