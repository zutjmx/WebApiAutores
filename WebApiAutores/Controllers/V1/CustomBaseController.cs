using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApiAutores.Controllers.V1
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        protected string ObtenerUsuario()
        {
            var usuarioClaim = HttpContext.User.Claims.Where(x => x.Type == "id").FirstOrDefault();
            var usuarioId = usuarioClaim.Value;
            return usuarioId;
        }
    }
}
