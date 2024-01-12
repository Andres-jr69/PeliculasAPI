using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Validaciones
{
    public class TipoArchivoValidacion : ValidationAttribute
    {
        private readonly string[] tipoValido;

        public TipoArchivoValidacion(string[] tipoValido)
        {
            this.tipoValido = tipoValido;
        }

        public TipoArchivoValidacion(GrupoTipoArchivo grupoTipoArchivo)
        {
            if (grupoTipoArchivo == GrupoTipoArchivo.Imagen)
            {
                tipoValido = new string[] { "image/jpeg", "image/jpg", "image/png", "image/gif" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is null)
            {
                return ValidationResult.Success;
            }

            IFormFile formFile = value as IFormFile;

            if (formFile is null)
            {
                return ValidationResult.Success;
            }

            if (!tipoValido.Contains(formFile.ContentType))
            {
                return new ValidationResult($"El tipo del archivo debe ser uno de los siguientes: {string.Join(", ", tipoValido) }");
            }

            return ValidationResult.Success;

        }

    }
}
