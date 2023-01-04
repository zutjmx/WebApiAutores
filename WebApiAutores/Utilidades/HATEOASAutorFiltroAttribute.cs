using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using WebApiAutores.DTOs;
using WebApiAutores.Servicios;

namespace WebApiAutores.Utilidades
{
    public class HATEOASAutorFiltroAttribute: HATEOASFiltroAttribute
    {
        private readonly GeneradorEnlaces generadorEnlaces;

        public HATEOASAutorFiltroAttribute(GeneradorEnlaces generadorEnlaces)
        {
            this.generadorEnlaces = generadorEnlaces;
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            var debeIncluir = DebeIncluirHATEOAS(context);
            
            if(!debeIncluir)
            {
                await next();
                return;
            }

            var resultado = context.Result as ObjectResult;            
            //var modelo = resultado.Value as AutorDto ?? throw new ArgumentNullException("Se esperaba una instancia de AutorDto");
            var autorDto = resultado.Value as AutorDto;

            if(autorDto == null)
            {
                var autoresDto = resultado.Value as List<AutorDto> ?? 
                    throw new ArgumentNullException("Se esperaba una instancia de AutorDto o List<AutorDto>");
                
                autoresDto.ForEach(async autor => await generadorEnlaces.GenerarEnlaces(autor));
                resultado.Value = autoresDto;
            } else
            {
                await generadorEnlaces.GenerarEnlaces(autorDto);
            }
            //await generadorEnlaces.GenerarEnlaces(modelo);

            await next();
        }
    }
}
