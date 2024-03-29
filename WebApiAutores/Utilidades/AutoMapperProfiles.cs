﻿using AutoMapper;
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

            CreateMap<Autor, AutorDtoConLibros>()
                .ForMember(autorDto => autorDto.Libros, opciones => opciones.MapFrom(MapAutorDtoLibros));

            CreateMap<LibroCreacionDto, Libro>()
                .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));

            CreateMap<Libro, LibroDto>();

            CreateMap<Libro, LibroDtoConAutores>()
                .ForMember(libroDto => libroDto.Autores, opciones => opciones.MapFrom(MapLibroDtoAutores));

            CreateMap<LibroPatchDto, Libro>().ReverseMap();

            CreateMap<ComentarioCreacionDto, Comentario>();
            CreateMap<Comentario, ComentarioDto>();
        }

        private List<LibroDto> MapAutorDtoLibros(Autor autor, AutorDto autorDto)
        {
            var resultado = new List<LibroDto>();

            if(autor.AutoresLibros == null) 
            {
                return resultado;
            }

            foreach (var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDto()
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo
                });
            }

            return resultado;
        }

        private List<AutorDto> MapLibroDtoAutores(Libro libro, LibroDto libroDto)
        {
            var resultado = new List<AutorDto>();

            if(libro.AutoresLibros == null)
            {
                return resultado;
            }

            foreach (var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDto()
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                });
            }

            return resultado;
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
