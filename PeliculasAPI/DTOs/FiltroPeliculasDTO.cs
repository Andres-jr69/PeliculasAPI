﻿namespace PeliculasAPI.DTOs
{
    public class FiltroPeliculasDTO
    {
        public int Pagina { get; set; } = 1;
        public int CantidadRegistroPorPagina { get; set; } = 10;
        public PaginacionDTO Paginacion
        {
            get { return new PaginacionDTO() { Pagina = Pagina, CantidadRegistroPorPagina = CantidadRegistroPorPagina }; }

        }

        public string Titulo { get; set; }
        public int GeneroId { get; set; }
        public bool EnCines { get; set; }
        public bool ProximoEstreno { get; set; }
        public string CampoOrdenar { get; set; }
        public bool OrdenAscendente { get; set; } = true;

    }
}
