using AutoMapper;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;
using NetTopologySuite.Geometries;


namespace PeliculasAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            /*El primero objeto sera la fuente de la informacion y 
              el segundo objeto va a ser hacia donde va la informacion  */
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            //Primero quien lo recibe y despues hacia donde lo quieres mapear
            CreateMap<GeneroCreacionDTO, Genero>();

            CreateMap<SalaDeCine, SalaDeCineDTO>()
                .ForMember(x => x.Latitud, x => x.MapFrom(y => y.Ubicacion)) ;

            CreateMap<SalaDeCineDTO, SalaDeCine>();

            //Primero quien lo recibe y despues hacia donde lo quieres mapear
            CreateMap<SalaDeCineCreacionDTO, SalaDeCine>();

            CreateMap<Actor, ActorTDO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x => x.Foto, opciones => opciones.Ignore());
            CreateMap<ActorPatchDTO, Actor>().ReverseMap();


            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(x => x.Poster, opciones => opciones.Ignore())
                /* x => x.PeliculasGeneros Es el destino */
                .ForMember(x => x.PeliculasGeneros, opcions => opcions.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.PeliculasActores, opcions => opcions.MapFrom(MapPeliculasActores));
            CreateMap<PeliculaPatcDTO, Pelicula>().ReverseMap();

            CreateMap<Pelicula, PeliculaDetallesDTO >()
                .ForMember(x => x.Generos, opcions => opcions.MapFrom(MapPeliculasGneros))
                .ForMember(x => x.Actores, opcions => opcions.MapFrom(MapPeliculasActores));
        }

        private List<ActorPeliculaDetalleDTO> MapPeliculasActores(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<ActorPeliculaDetalleDTO>();
            if (pelicula.PeliculasActores is null)
            {
                return resultado;
            }
            foreach (var actorPelicula in pelicula.PeliculasActores)
            {
                resultado.Add(new ActorPeliculaDetalleDTO() 
                { 
                    ActorId = actorPelicula.ActorId, 
                    Personaje = actorPelicula.Personaje, 
                    NombrePersona = actorPelicula.Actor.Nombre
                });
            }

            return resultado;
        }

        private List<GeneroDTO> MapPeliculasGneros(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<GeneroDTO>();
            if (pelicula.PeliculasGeneros is null)
            {
                return resultado;
            }
            foreach (var generoPelicula in pelicula.PeliculasGeneros)
            {
                resultado.Add(new GeneroDTO() { Id = generoPelicula.GeneroId, Nombre = generoPelicula.Genero.Nombre });
            }

            return resultado;

        }
        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();
            if (peliculaCreacionDTO.GenerosIDs == null)
            {
                //Retornar el listado vasio 
                return resultado;
            }

            foreach (var id in peliculaCreacionDTO.GenerosIDs)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }
            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActores(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();
            if (peliculaCreacionDTO.Actores is null )
            {
                return resultado;
            }

            foreach (var actor in peliculaCreacionDTO.Actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actor.ActorId, Personaje = actor.Personaje });
            }

            return resultado;

        }
    }
}
