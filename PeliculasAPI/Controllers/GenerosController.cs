using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenerosController(ApplicationDbContext context,
            IMapper mapper) 
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO >>> Get()
        {
            var entidades = await context.Generos.ToListAsync();
            var dtos = mapper.Map<List<GeneroDTO>>(entidades);
            return dtos;
        }
        [HttpGet("{id:int}", Name = "obtenerGenero")]
        public async Task<ActionResult<GeneroDTO>> Get(int id)
        {
            var genero = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);
            if (genero is null)
            {
                return NotFound();
            }

            var dto = mapper.Map<GeneroDTO>(genero);
            return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Pos([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            //var existeGeneroConEseNombre = await context.Generos.AnyAsync(x => x.Nombre == generoCreacionDTO.Nombre);

            //if (existeGeneroConEseNombre )
            //{
            //    return BadRequest($"ya existe un autor con el nombre {generoCreacionDTO.Nombre}");
            //}

            //if (!ModelState.IsValid)
            //{
            //    return NotFound();
            //}

            var genero = mapper.Map<Genero>(generoCreacionDTO);
            context.Add(genero);
            await context.SaveChangesAsync();

            var generoDTO = mapper.Map<GeneroDTO>(genero);
            return new CreatedAtRouteResult("obtenerGenero", new { id = generoDTO.Id }, generoDTO);
            
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put([FromBody] GeneroCreacionDTO generoCreacionDTO, int id)
        {
            var existe = await context.Generos.AnyAsync(x => x.Id == id);
            if (!existe)
            {
                return NotFound();
            }
            var genero = mapper.Map<Genero>(generoCreacionDTO);
            genero.Id = id;

            context.Entry(genero).State  = EntityState.Modified;
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Generos.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Genero() { Id = id });
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}
