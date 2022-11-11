using System.ComponentModel.DataAnnotations;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Libro
    {
        public int Id { get; set; }

        [StringLength(maximumLength: 250, ErrorMessage = "El campo {0} no puede tener más de {1} caracteres")]
        [PrimeraLetraMayuscula]
        public string Titulo { get; set; }
    }
}
