namespace PeliculasAPI.Servicios
{
    public class AlmacenadorArchivoLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AlmacenadorArchivoLocal(IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task BorrarArchivo(string ruta, string contenedor)
        {
            throw new NotImplementedException();
        }

        public Task<string> EditarArchivo(IFormFile archivo, string contenedor, string ruta)
        {
            throw new NotImplementedException();
        }

        public Task<string> GuardarArchivo(string contenedor, IFormFile archivo)
        {
            throw new NotImplementedException();
        }
    }
}
