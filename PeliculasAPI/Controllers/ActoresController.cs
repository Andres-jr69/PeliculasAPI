using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using PeliculasAPI.Migrations;
using PeliculasAPI.Servicios;
using System.Diagnostics;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;

        public ActoresController(ApplicationDbContext context,
            IMapper mapper,
            IAlmacenadorArchivos almacenadorArchivos)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorTDO>>> Get()
        {
            var entidades = await context.Actores.ToListAsync();
            var dtos = mapper.Map<List<ActorTDO>>(entidades);
            return dtos;
        }
        [HttpGet("{id:int}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorTDO>> Get(int id)
        {
            var Actor = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);
            if (Actor is null)
            {
                return NotFound();
            }

            var dto = mapper.Map<ActorTDO>(Actor);
            return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            

            //var actor = mapper.Map<Quien lo guara>(Quien lo recibe);
           
            var actor = mapper.Map<Actor>(actorCreacionDTO);

            if (actorCreacionDTO.Foto != null)
            {
                actor.Foto = await almacenadorArchivos.GuardarArchivo(contenedor, actorCreacionDTO.Foto);


            }

            context.Add(actor);
            //await context.SaveChangesAsync();

            var actorTDO = mapper.Map<ActorTDO>(actor);
            return new CreatedAtRouteResult("obtenerActor", new { id =  actorTDO.Id }, actorTDO);
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromForm] ActorCreacionDTO actorCreacionDTO, int id)
        {
            var existe = await context.Actores.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            var actor = mapper.Map<Actor>(actorCreacionDTO);
            actor.Id = id;

            context.Entry(actor).State = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Actores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Actor() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
