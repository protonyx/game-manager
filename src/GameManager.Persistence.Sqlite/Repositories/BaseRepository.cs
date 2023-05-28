using GameManager.Application.Data;
using Microsoft.EntityFrameworkCore;

namespace GameManager.Server.Data;

public class BaseRepository<T> : IAsyncRepository<T> where T : class
{
    protected readonly GameContext _context;

    public BaseRepository(GameContext context)
    {
        _context = context;
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
    {
        T? t = await _context.Set<T>().FindAsync(id);
        
        return t;
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();

        return entity;
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteByIdAsync(Guid id)
    {
        T? t = await GetByIdAsync(id);
        if (t != null)
            await DeleteAsync(t);
    }
}