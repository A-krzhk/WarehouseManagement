using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync()
    {
        return await _dbContext.Set<T>().ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbContext.Set<T>().Where(predicate).ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>>? predicate = null, 
                                                Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, 
                                                string? includeString = null, 
                                                bool disableTracking = true)
    {
        IQueryable<T> query = _dbContext.Set<T>();
        
        if (disableTracking) query = query.AsNoTracking();
        
        if (!string.IsNullOrWhiteSpace(includeString)) query = query.Include(includeString);
        
        if (predicate != null) query = query.Where(predicate);
        
        if (orderBy != null)
            return await orderBy(query).ToListAsync();
        
        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>>? predicate = null, 
                                                Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, 
                                                List<Expression<Func<T, object>>>? includes = null, 
                                                bool disableTracking = true)
    {
        IQueryable<T> query = _dbContext.Set<T>();
        
        if (disableTracking) query = query.AsNoTracking();
        
        if (includes != null) query = includes.Aggregate(query, (current, include) => current.Include(include));
        
        if (predicate != null) query = query.Where(predicate);
        
        if (orderBy != null)
            return await orderBy(query).ToListAsync();
        
        return await query.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbContext.Set<T>().FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbContext.Set<T>().AddAsync(entity);
        await _dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        _dbContext.Entry(entity).State = EntityState.Modified;
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbContext.Set<T>().Remove(entity);
        await _dbContext.SaveChangesAsync();
    }
}