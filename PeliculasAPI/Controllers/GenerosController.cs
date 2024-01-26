using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController : CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenerosController(ApplicationDbContext context,
            IMapper mapper) 
            :base (context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO >>> Get()
        {
            //Primero quien lo recibe y luego hacia donde va
            return await Get<Genero,  GeneroDTO>();
            //var entidades = await context.Generos.ToListAsync();
            //var dtos = mapper.Map<List<GeneroDTO>>(entidades);
            //return dtos;
        }
        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            //Primero quien lo recibe y luego hacia donde va
            return await Get<Genero, GeneroDTO>(id);
            //var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);
            //if (genero is null)
            //{
            //    return NotFound();
            //}

            //var dto = mapper.Map<GeneroDTO>(genero);
            //return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Pos([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {

            return await Post<GeneroCreacionDTO, Genero, GeneroDTO>(generoCreacionDTO, "obtenerGenero");
            //var existeGeneroConEseNombre = await context.Generos.AnyAsync(x => x.Nombre == generoCreacionDTO.Nombre);

            //if (existeGeneroConEseNombre )
            //{
            //    return BadRequest($"ya existe un autor con el nombre {generoCreacionDTO.Nombre}");
            //}

            //if (!ModelState.IsValid)
            //{
            //    return NotFound();
            //}

            //var genero = mapper.Map<Genero>(generoCreacionDTO);
            //context.Add(genero);
            //await context.SaveChangesAsync();

            //var generoDTO = mapper.Map<GeneroDTO>(genero);
            //return new CreatedAtRouteResult("obtenerGenero", new { id = generoDTO.Id }, generoDTO);
            
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromBody] GeneroCreacionDTO generoCreacionDTO, int id)
        {
            var existe = await context.Generos.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            return await Put<GeneroCreacionDTO, Genero>(id, generoCreacionDTO);
            //var genero = mapper.Map<Genero>(generoCreacionDTO);
            //genero.Id = id;

            //context.Entry(genero).State  = EntityState.Modified;
            //await context.SaveChangesAsync();
            //return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Genero>(id);
            //var existe = await context.Generos.AnyAsync(x => x.Id == id);

            //if (!existe)
            //{
            //    return NotFound();
            //}

            //context.Remove(new Genero() { Id = id });
            //await context.SaveChangesAsync();
            //return NoContent();
        }
    }
}
