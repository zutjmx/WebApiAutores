namespace WebApiAutores.DTOs
{
    public class PaginacionDto
    {
        public int Pagina { get; set; } = 1;
        private int registrosPorPagina = 10;
        private readonly int cantidadMaximaPorPagina = 50;

        public int RegistrosPorPagina 
        { 
            get 
            { 
                return registrosPorPagina;
            }
            set 
            { 
                registrosPorPagina = (value>cantidadMaximaPorPagina)?cantidadMaximaPorPagina:value;
            }
        }

    }
}
