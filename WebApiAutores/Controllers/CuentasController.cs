using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApiAutores.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;

        public CuentasController(
            UserManager<IdentityUser> userManager, 
            IConfiguration configuration,
            SignInManager<IdentityUser> signInManager
            )
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.signInManager = signInManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(CredencialesUsuario credencialesUsuario)
        {
            var resultado = await signInManager.PasswordSignInAsync(credencialesUsuario.Email,
                                                                    credencialesUsuario.Password,
                                                                    isPersistent: false, 
                                                                    lockoutOnFailure: false);
            if(resultado.Succeeded)
            {
                return ConstruirToken(credencialesUsuario);
            } else
            {
                return BadRequest($"Login incorrecto para el usuario: {credencialesUsuario.Email}");
            }
            
        }

        // POST api/<CuentasController>/registrar
        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar([FromBody] CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser { UserName = credencialesUsuario.Email, Email = credencialesUsuario.Email };
            var resultado = await userManager.CreateAsync(usuario,credencialesUsuario.Password);
            
            if(resultado.Succeeded)
            {
                return ConstruirToken(credencialesUsuario);
            } else 
            {
                return BadRequest(resultado.Errors);
            }
            
        }

        [HttpGet("RenovarToken")]
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<RespuestaAutenticacion> Renovar()
        {
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;

            var credencialesUsuario = new CredencialesUsuario 
            { 
                Email = email 
            };

            return ConstruirToken(credencialesUsuario);
        }

        private RespuestaAutenticacion ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("email",credencialesUsuario.Email)
            };

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llaveJwt"]));
            var credenciales = new SigningCredentials(llave,SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddHours(1);

            var securityToken = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: expiracion,
                    signingCredentials: credenciales
                );

            return new RespuestaAutenticacion() {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion,
            };

        }

    }
}
