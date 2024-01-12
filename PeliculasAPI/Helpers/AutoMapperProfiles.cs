using AutoMapper;
using PeliculasAPI.DTOs;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            //Primero quien lo recibe y despues hacia donde lo quieres mapear
            CreateMap<GeneroCreacionDTO, Genero>();
            CreateMap<Actor, ActorTDO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>();

        }
    }
}
