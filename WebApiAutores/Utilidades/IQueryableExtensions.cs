using WebApiAutores.DTOs;

namespace WebApiAutores.Utilidades
{
    public static class IQueryableExtensions
    {
        public static IQueryable<T> Paginar<T>(this IQueryable<T> queryable, PaginacionDto paginacionDto)
        {
            return queryable
                .Skip((paginacionDto.Pagina-1)*paginacionDto.RegistrosPorPagina)
                .Take(paginacionDto.RegistrosPorPagina);
        }
    }
}
