using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;
using WebApiAutores.Servicios;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers.V1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        public CuentasController(
            UserManager<IdentityUser> userManager,
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager,
            IDataProtectionProvider dataProtectionProvider,
            HashService hashService
            )
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = dataProtectionProvider.CreateProtector(this.configuration["unDato"]);
        }

        //[HttpGet("hash/{cadena}")]
        //public ActionResult ObtenerHash(string cadena) 
        //{
        //    var primerHash = this.hashService.Hash(cadena);
        //    var segudoHash = this.hashService.Hash(cadena);
        //    return Ok(new
        //    {
        //        cadena,
        //        primerHash,
        //        segudoHash,
        //    });
        //}

        //[HttpGet("encriptar/{cadena}")]
        //public ActionResult Encriptar(string cadena)
        //{
        //    var cadenaCifrada = this.dataProtector.Protect(cadena);
        //    var cadenaDescifrada = this.dataProtector.Unprotect(cadenaCifrada);

        //    return Ok(new
        //    {
        //        cadena,
        //        cadenaCifrada,
        //        cadenaDescifrada
        //    });
        //}

        //[HttpGet("encriptarPorTiempo/{cadena}")]
        //public ActionResult EncriptarPorTiempo(string cadena)
        //{
        //    var protectorLimitadoPorTiempo = this.dataProtector.ToTimeLimitedDataProtector();

        //    var cadenaCifrada = protectorLimitadoPorTiempo.Protect(cadena,lifetime: TimeSpan.FromSeconds(5));
        //    //Thread.Sleep(TimeSpan.FromSeconds(6)); //Para probar el vencimiento del cifrado.
        //    var cadenaDescifrada = protectorLimitadoPorTiempo.Unprotect(cadenaCifrada);

        //    return Ok(new
        //    {
        //        cadena,
        //        cadenaCifrada,
        //        cadenaDescifrada
        //    });
        //}

        [HttpPost("login", Name = "loginUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email,
                                                                    credencialesUsuario.Password,
                                                                    isPersistent: false,
                                                                    lockoutOnFailure: false);
            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest($"Login incorrecto para el usuario: {credencialesUsuario.Email}");
            }

        }

        // POST api/<CuentasController>/registrar
        [HttpPost("registrar", Name = "registrarUsuario")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar([FromBody] CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser { UserName = credencialesUsuario.Email, Email = credencialesUsuario.Email };
            var resultado = await userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(credencialesUsuario);
            }
            else
            {
                return BadRequest(resultado.Errors);
            }

        }

        [HttpGet("RenovarToken", Name = "renovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var credencialesUsuario = new CredencialesUsuario
            {
                Email = email
            };

            return await ConstruirToken(credencialesUsuario);
        }

        [HttpPost("HacerAdmin", Name = "hacerAdmin")]
        public async Task<ActionResult> HacerAdmin(EditarAdminDto editarAdminDto)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDto.Email);
            await userManager.AddClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }

        [HttpPost("RemoverAdmin", Name = "removerAdmin")]
        public async Task<ActionResult> RemoverAdmin(EditarAdminDto editarAdminDto)
        {
            var usuario = await userManager.FindByEmailAsync(editarAdminDto.Email);
            await userManager.RemoveClaimAsync(usuario, new Claim("esAdmin", "1"));
            return NoContent();
        }

        private async Task<RespuestaAutenticacion> ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("email",credencialesUsuario.Email)
            };

            var usuario = await userManager.FindByEmailAsync(credencialesUsuario.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDB);

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llaveJwt"]));
            var credenciales = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddHours(1);

            var securityToken = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: expiracion,
                    signingCredentials: credenciales
                );

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion,
            };

        }

    }
}
