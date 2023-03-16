using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiAutores.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FacturasController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public FacturasController(ApplicationDbContext context)
        {
            this.context = context;
        }

        // GET: api/<FacturasController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<FacturasController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<FacturasController>
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] PagarFacturaDTO pagarFacturaDTO)
        {
            var facturaDB = await context.Facturas
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == pagarFacturaDTO.FacturaId);
            
            if (facturaDB == null)
            {
                return NotFound();
            }

            if(facturaDB.Pagada)
            {
                return BadRequest("La factura ya se pagó");
            }

            facturaDB.Pagada = true;
            await context.SaveChangesAsync();

            var hayFacturasPendientesVencidas = await context.Facturas
                .AnyAsync(x => x.UsuarioId == facturaDB.UsuarioId && !x.Pagada && x.FechaLimiteDePago < DateTime.Today);

            if (!hayFacturasPendientesVencidas)
            {
                facturaDB.Usuario.MalaPaga = false;
                await context.SaveChangesAsync();
            }

            return NoContent();
        }

        // PUT api/<FacturasController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<FacturasController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
