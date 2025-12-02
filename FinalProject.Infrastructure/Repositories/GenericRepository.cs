using System.Linq.Expressions;
using FinalProject.Domain.Interfaces;
using FinalProject.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinalProject.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly LukitasDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    // Límite máximo de registros por página para proteger la BD
    private const int MaxPageSize = 100;

    public GenericRepository(LukitasDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Obtiene todos los registros con paginación obligatoria.
    /// NUNCA carga toda la tabla en memoria.
    /// </summary>
    public async Task<PaginatedResult<T>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        // Validar y limitar parámetros
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        
        var totalCount = await _dbSet.CountAsync();
        var items = await _dbSet
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Obtiene registros filtrados con paginación obligatoria.
    /// </summary>
    public async Task<PaginatedResult<T>> FindAsync(Expression<Func<T, bool>> predicate, int page = 1, int pageSize = 20)
    {
        // Validar y limitar parámetros
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, MaxPageSize);
        
        var query = _dbSet.Where(predicate);
        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Retorna IQueryable para queries personalizadas.
    /// El consumidor es responsable de aplicar paginación con Skip/Take.
    /// </summary>
    public IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }

    /// <summary>
    /// Retorna IQueryable filtrado para queries personalizadas.
    /// </summary>
    public IQueryable<T> Query(Expression<Func<T, bool>> predicate)
    {
        return _dbSet.Where(predicate);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    public async Task<int> CountAsync()
    {
        return await _dbSet.CountAsync();
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).CountAsync();
    }
}
