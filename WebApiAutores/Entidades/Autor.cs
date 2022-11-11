﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApiAutores.Validaciones;

namespace WebApiAutores.Entidades
{
    public class Autor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength:60,ErrorMessage ="El campo {0} no puede tener más de {1} caracteres")]
        [PrimeraLetraMayuscula]
        public string Nombre { get; set; }

    }
}
