using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.DTOs
{
    public class GeneroCreacionDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        //[StringLength(maximumLength: 120, ErrorMessage = "El campo {0} no debe tener mas de {1} caracteres")]
        [StringLength(40)]
        public string Nombre { get; set; }
    }
}
