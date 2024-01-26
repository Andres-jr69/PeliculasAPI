using AutoMapper;
using Azure;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using PeliculasAPI.Migrations;
using PeliculasAPI.Servicios;
using System.Diagnostics;
using System.Reflection;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        //Carpeta donde se guardara la foto de los actores
        private readonly string contenedor = "actores";

        public ActoresController(ApplicationDbContext context,
            IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos
            ) :base(context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorTDO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            return await Get<Actor, ActorTDO>(paginacionDTO);
            //Paginacion
            //var queryable = context.Actores.AsQueryable();
            //await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadRegistroPorPagina);
            //var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();

            //mapeo
            //Recuerda ir a AutoMapperProfiles para que esto sea valido
            //var dtos = mapper.Map<List<ActorTDO>>(entidades);
            //return dtos;
        }

        

        [HttpGet("{id:int}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorTDO>> Get(int id)
        {
            return await Get<Actor, ActorTDO>(id);
            //var Actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            //if (Actor is null)
            //{
            //    return NotFound();
            //}
            ////Recuerda ir a AutoMapperProfiles para que esto sea valido
            //var dto = mapper.Map<ActorTDO>(Actor);
            //return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {


            //var actor = mapper.Map<Quien lo guara>(Quien lo recibe);
            //Recuerda ir a AutoMapperProfiles para que esto sea valido
            var actor = mapper.Map<Actor>(actorCreacionDTO);

            if (actorCreacionDTO.Foto != null)
            {
                actor.Foto = await almacenadorArchivos.GuardarArchivo(contenedor, actorCreacionDTO.Foto);


            }

            context.Add(actor);
            await context.SaveChangesAsync();

            var actorTDO = mapper.Map<ActorTDO>(actor);
            return new CreatedAtRouteResult("obtenerActor", new { id =  actorTDO.Id }, actorTDO);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromForm] ActorCreacionDTO actorCreacionDTO, int id)
        {
            var actorDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (actorDB == null)
            {
                return NotFound();
            }
            //Recuerda ir a AutoMapperProfiles para que esto sea valido
            actorDB = mapper.Map(actorCreacionDTO, actorDB); //Toma los campos del primero y los mapea al segundo
            //al utilizar SaveChangesAsync, solo los datos distintos se actualizaran

            if (actorCreacionDTO.Foto != null)
            {
                actorDB.Foto = await almacenadorArchivos.EditarArchivo(actorCreacionDTO.Foto, contenedor, actorDB.Foto);
            }

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        /*
         path: "/nombre" --Nombre de la propieda a actualizar
        op : "replace" --Significa que haras una replace/sustitucion
        value: "Nombre nuevo" --Valor que vas a colocar
         */
        public async Task<ActionResult> Patch(int id,[FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {

            return await Patch<Actor, ActorPatchDTO>(id, patchDocument);
            //if (patchDocument is null)
            //{
            //    return BadRequest();
            //}
            //var entidadDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            //if (entidadDB == null)
            //{
            //    return NotFound();
            //}
            //Recuerda ir a AutoMapperProfiles para que esto sea valido
            //var entidadDTO = mapper.Map<ActorPatchDTO>(entidadDB);
            //lo que se va a actualizar
            //patchDocument.ApplyTo(entidadDTO, ModelState);

            //por  si pasa algo mientra se aplica, Como lo sabemos?
            //var esValido = TryValidateModel(entidadDTO);
            //Para que sea conciente de los errores
            //if (!esValido)
            //{
            //    return BadRequest(ModelState);
            //}
            //mapper.Map(entidadDTO, entidadDB);
            //await context.SaveChangesAsync();
            //return NoContent();
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Actor>(id); 
            //var existe = await context.Actores.AnyAsync(x => x.Id == id);

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
