using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using PeliculasAPI.Migrations;
using PeliculasAPI.Servicios;
using System.Linq.Dynamic.Core;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PeliculasAPI.Controllers
{
    [Route("api/peliculas")]
    [ApiController]
    public class PeliculasController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ILogger<PeliculasController> logger;
        private readonly string contenedor = "pelicuas";
        public PeliculasController(ApplicationDbContext context,
            IMapper mapper, 
            IAlmacenadorArchivos almacenadorArchivos,
            ILogger<PeliculasController> logger)
            :base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.logger = logger;
        }

        // GET: api/<PeliculasController>
        [HttpGet]
        public async Task<ActionResult<PeliculasIndexDTO>> Get()
        {
            var top = 5;
            var hoy = DateTime.Today;

            var proximosExtrenos = await context.Peliculas
                .Where(x => x.FechaEstreno > hoy)
                .OrderBy(x => x.FechaEstreno)
                .Take(top).ToListAsync();

            var enCines = await context.Peliculas
                .Where(x => x.EnCines)
                .Take(top)
                .ToListAsync();

            var resultado = new PeliculasIndexDTO();
            resultado.FuturosEstreno = mapper.Map<List<PeliculaDTO>>(proximosExtrenos);
            resultado.EnCines = mapper.Map<List<PeliculaDTO>>(enCines);
            return resultado;

            //var peliculas = await context.Peliculas.ToListAsync();
            //return mapper.Map<List<PeliculaDTO>>(peliculas);
        }
        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] FiltroPeliculasDTO filtroPeliculasDTO)
        {
            var peliculasQueryable = context.Peliculas.AsQueryable();

            //Si el cliente nos envia un titulo queremos filtrar por ese titulo
            if (!string.IsNullOrEmpty(filtroPeliculasDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculasDTO.Titulo));
            }

            if (filtroPeliculasDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines);
            }

            if (filtroPeliculasDTO.ProximoEstreno)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > hoy);
            }

            if (filtroPeliculasDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.PeliculasGeneros.Select(x => x.GeneroId)
                .Contains(filtroPeliculasDTO.GeneroId)
                );
                    
            }

            //Esto hace que se ordene por el compa que el ususario diga
            if (!string.IsNullOrEmpty(filtroPeliculasDTO.CampoOrdenar))
            {
                var tipoOrden = filtroPeliculasDTO.OrdenAscendente ? "ascending" : "descending";

                try
                {
                    peliculasQueryable = peliculasQueryable.OrderBy($"{filtroPeliculasDTO.CampoOrdenar} {tipoOrden}");

                }
                catch(Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                }
                //if (filtroPeliculasDTO.CampoOrdenar == "titulo")
                //{
                //    if (filtroPeliculasDTO.OrdenAscendente)
                //    {
                //        peliculasQueryable = peliculasQueryable.OrderBy(x => x.Titulo);

                //    }
                //    else
                //    {
                //        peliculasQueryable = peliculasQueryable.OrderByDescending(x => x.Titulo);

                //    }
                //}
            }

            await HttpContext.InsertarParametrosPaginacion(peliculasQueryable, 
                filtroPeliculasDTO.CantidadRegistroPorPagina);

            var peliculas = await peliculasQueryable.Paginar(filtroPeliculasDTO.Paginacion).ToListAsync();   

            return mapper.Map<List<PeliculaDTO>>(peliculas);

        }

        // GET api/<PeliculasController>/5
        [HttpGet("{id:int}", Name = "obtenerPelicula")]
        public async Task<ActionResult<PeliculaDetallesDTO>> Get(int id)
        {
            var pelicula = await context.Peliculas
                .Include(x => x.PeliculasActores).ThenInclude(x => x.Actor)
                .Include(x => x.PeliculasGeneros).ThenInclude(x => x.Genero)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (pelicula is null)
            {
                return NotFound();
            }
            pelicula.PeliculasActores = pelicula.PeliculasActores.OrderBy(x => x.Orden).ToList();

            return mapper.Map<PeliculaDetallesDTO>(pelicula);

        }

        // POST api/<PeliculasController>
        [HttpPost]
        public async Task<ActionResult> Post([FromForm]PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula =  mapper.Map<Pelicula>(peliculaCreacionDTO);


            if (peliculaCreacionDTO.Poster != null)
            {
                pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenedor, peliculaCreacionDTO.Poster);

            }
            AsignarOrdenActores(pelicula);
            context.Add(pelicula);
            await context.SaveChangesAsync();
            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return new CreatedAtRouteResult("obtenerPelicula", new { id = pelicula.Id }, peliculaDTO);
        }

        private void AsignarOrdenActores(Pelicula pelicula)
        {
            if (pelicula.PeliculasActores != null)
            {
                for (int i = 0; i < pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }
            }
        }

        // PUT api/<PeliculasController>/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO )
        {
            var peliculaDB = await context.Peliculas
                .Include(x => x.PeliculasActores)
                .Include(x => x.PeliculasGeneros)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (peliculaDB == null)
            {
                return NotFound();
            }

            peliculaDB = mapper.Map(peliculaCreacionDTO, peliculaDB); //Toma los campos del primero y los mapea al segundo
            //al utilizar SaveChangesAsync, solo los datos distintos se actualizaran

            if (peliculaCreacionDTO.Poster != null)
            {
                peliculaDB.Poster = await almacenadorArchivos.EditarArchivo(peliculaCreacionDTO.Poster, contenedor, peliculaDB.Poster);
            }
            AsignarOrdenActores(peliculaDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        /*
         path: "/nombre" --Nombre de la propieda a actualizar
        op : "replace" --Significa que haras una replace/sustitucion
        value: "Nombre nuevo" --Valor que vas a colocar
         */
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPatcDTO> patchDocument)
        {
            return await Patch<Pelicula, PeliculaPatcDTO>(id, patchDocument);

            //if (patchDocument is null)
            //{
            //    return BadRequest();
            //}
            //var entidadDB = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            //if (entidadDB == null)
            //{
            //    return NotFound();
            //}

            //var entidadDTO = mapper.Map<PeliculaPatcDTO>(entidadDB);
            //lo que se va a actualizar
            //patchDocument.ApplyTo(entidadDTO, ModelState);

            //por  si pasa algo mientra se aplica, Como lo sabemos?
            //var esValido = TryValidateModel(entidadDTO);
            //Para que sea conciente de los errores
            //if (!esValido)
            //{
            //    return BadRequest(ModelState);
            //}
            //Toma los campos del primero y los mapea al segundo
            //mapper.Map(entidadDTO, entidadDB);
            //await context.SaveChangesAsync();
            //return NoContent();
        }

        // DELETE api/<PeliculasController>/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Pelicula>(id);

            //var existe = await context.Peliculas.AnyAsync(x => x.Id == id);

            //if (!existe)
            //{
            //    return NotFound();
            //}

            //context.Remove(new Pelicula() { Id = id });
            //await context.SaveChangesAsync();
            //return NoContent();
        }
    }
}
