using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;

namespace PeliculasAPI.Controllers
{
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public CustomBaseController(
            ApplicationDbContext context,
            IMapper mapper
            )
        {
            this.context = context;
            this.mapper = mapper;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>() where TEntidad : class
        {
            var entidades = await context.Set<TEntidad>().ToListAsync(); 
            var dtos = mapper.Map<List<TDTO>>( entidades );
            return dtos;
        }

        protected async Task<List<TDTO>> Get<TEntidad, TDTO>
            (PaginacionDTO paginacionDTO) where TEntidad : class
        {
            //Paginacion
            var queryable = context.Set<TEntidad>().AsQueryable();
            await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadRegistroPorPagina);
            var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();

            //mapeo
            //Recuerda ir a AutoMapperProfiles para que esto sea valido
            var dtos = mapper.Map<List<TDTO>>(entidades);
            return dtos;
        }

        protected async Task<ActionResult<TDTO>> Get<TEntidad, TDTO>(int id) where TEntidad : class, IId
        {
            var entidad = await context.Set<TEntidad>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }
            return mapper.Map<TDTO>( entidad );
        }

        protected async Task<ActionResult> Post<TCreacion, TEntidad, TLectura>
            (TCreacion creacionDTo, string nombreRuta) where TEntidad: class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacionDTo);
            context.Add(entidad);
            await context.SaveChangesAsync();

            var dtoLectura = mapper.Map<TLectura>(entidad);
            return new CreatedAtRouteResult(nombreRuta, new { id = entidad.Id }, dtoLectura);
        }

        protected async Task<ActionResult> Put<TCreacion, TEntidad>
            (int id, TCreacion creacionDTO) where TEntidad: class, IId
        {
            var entidad = mapper.Map<TEntidad>(creacionDTO);
            entidad.Id = id;

            context.Entry(entidad).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult> Patch<TEntidad, TDTO>
            (int id, JsonPatchDocument<TDTO> patchDocument) where TDTO : class where TEntidad: class, IId
        {
            if (patchDocument is null)
            {
                return BadRequest();
            }
            var entidadDB = await context.Set<TEntidad>().FirstOrDefaultAsync(x => x.Id == id);
            if (entidadDB == null)
            {
                return NotFound();
            }
            //Recuerda ir a AutoMapperProfiles para que esto sea valido
            var entidadDTO = mapper.Map<TDTO>(entidadDB);
            //lo que se va a actualizar
            patchDocument.ApplyTo(entidadDTO, ModelState);

            //por  si pasa algo mientra se aplica, Como lo sabemos?
            var esValido = TryValidateModel(entidadDTO);
            //Para que sea conciente de los errores
            if (!esValido)
            {
                return BadRequest(ModelState);
            }
            mapper.Map(entidadDTO, entidadDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult> Delete<TEntidad>
            (int id) where TEntidad: class, IId,   new()
        {
            var existe = await context.Set<TEntidad>().AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new TEntidad() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
