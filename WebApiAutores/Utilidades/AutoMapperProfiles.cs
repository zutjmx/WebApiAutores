using AutoMapper;
using WebApiAutores.DTOs;
using WebApiAutores.Entidades;

namespace WebApiAutores.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorCreacionDto, Autor>();            
            CreateMap<Autor, AutorDto>();

            CreateMap<LibroCreacionDto, Libro>()
                .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));

            CreateMap<Libro, LibroDto>();
            
            CreateMap<ComentarioCreacionDto, Comentario>();
            CreateMap<Comentario, ComentarioDto>();
        }

        private List<AutorLibro> MapAutoresLibros(LibroCreacionDto libroCreacionDto, Libro libro) 
        { 
            var listado = new List<AutorLibro>();

            if(libroCreacionDto.AutoresIds == null)
            {
                return listado;
            }

            foreach (var autorId in libroCreacionDto.AutoresIds)
            {
                listado.Add(new AutorLibro() { AutorId = autorId });
            }

            return listado;
        }

    }
}
