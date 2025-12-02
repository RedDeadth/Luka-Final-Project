using System.Linq.Expressions;

namespace FinalProject.Domain.Interfaces;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByIdAsync(int id);
    
    /// <summary>
    /// Obtiene todos los registros con paginación obligatoria para evitar cargar toda la tabla en memoria.
    /// </summary>
    /// <param name="page">Número de página (1-based)</param>
    /// <param name="pageSize">Tamaño de página (máximo 100)</param>
    Task<PaginatedResult<T>> GetAllAsync(int page = 1, int pageSize = 20);
    
    /// <summary>
    /// Obtiene registros filtrados con paginación.
    /// </summary>
    Task<PaginatedResult<T>> FindAsync(Expression<Func<T, bool>> predicate, int page = 1, int pageSize = 20);
    
    /// <summary>
    /// Obtiene el primer registro que cumple el predicado o null.
    /// </summary>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    
    /// <summary>
    /// Retorna IQueryable para queries personalizadas (el consumidor es responsable de paginar).
    /// </summary>
    IQueryable<T> Query();
    
    /// <summary>
    /// Retorna IQueryable filtrado para queries personalizadas.
    /// </summary>
    IQueryable<T> Query(Expression<Func<T, bool>> predicate);
    
    Task AddAsync(T entity);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync();
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
}

/// <summary>
/// Resultado paginado para evitar cargar toda la tabla en memoria.
/// </summary>
public class PaginatedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;
}
