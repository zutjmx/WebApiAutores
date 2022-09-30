using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor: IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:60,ErrorMessage ="El campo {0} no puede tener más de {1} caracteres")]
        //[PrimeraLetraMayuscula]
        public string Nombre { get; set; }

        //[Range(18,120,ErrorMessage ="La {0} debe de estar en el rango [{1}, {2}]")]
        //[NotMapped]
        //public int Edad { get; set; }

        //[CreditCard(ErrorMessage ="Se requiere un {0} de TDC válido")]
        //[NotMapped]
        //public string TarjetaDeCredito { get; set; }
        
        //[Url(ErrorMessage = "Se requiere una {0} válida")]
        //[NotMapped]
        //public string UrlAutor { get; set; }

        //[NotMapped]
        //public int Menor { get; set; }

        //[NotMapped]
        //public int Mayor { get; set; }
        public List<Libro> Libros { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!String.IsNullOrEmpty(Nombre))
            {
                string PrimeraLetra = Nombre[0].ToString();
                if (!PrimeraLetra.Equals(PrimeraLetra.ToUpper()))
                {
                    yield return new ValidationResult(
                        "La primera letra debe de ser mayúscula", 
                        new string[] {nameof(Nombre)}
                    );
                }
            }

            //if (Menor > Mayor)
            //{
            //    yield return new ValidationResult(
            //            "El valor del campo Menor no puede ser mayor al campo Mayor",
            //            new string[] { nameof(Menor) }
            //        );
            //}

        }
    }
}
