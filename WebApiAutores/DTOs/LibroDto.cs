namespace WebApiAutores.DTOs
{
    public class LibroDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public List<AutorDto> Autores { get; set; }
        public List<ComentarioDto> Comentarios { get; set; }
    }
}
