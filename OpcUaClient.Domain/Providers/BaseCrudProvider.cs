using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using OpcUaClient.Domain.Exceptions;
using OpcUaClient.Domain.Interfaces;
using OpcUaClient.Domain.Interfaces.Providers;
using OpcUaClient.Domain.Models;

namespace OpcUaClient.Domain.Providers;

public class BaseCrudProvider<T> : ICrudProvider<T> where T : Entity
{
    protected readonly IServiceDbContext _dbContext;

    public BaseCrudProvider(IServiceDbContext context)
    {
        _dbContext = context;
    }


    public async Task<List<T>> GetAll(Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool asNoTracking = false)
    {
        var query = _dbContext.Set<T>().AsQueryable();

        if (includes != null)
        {
            query = includes(query);
        }

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.ToListAsync();
    }

    public async Task<T> Get(Guid id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool asNoTracking = false)
    {
        var query = _dbContext.Set<T>().AsQueryable();

        if (includes != null)
        {
            query = includes(query);
        }

        if (asNoTracking)
            query = query.AsNoTracking();
        
        var result = await query.FirstOrDefaultAsync(e => e.Id == id);
        
        return result ?? throw new EntityNotFoundException(typeof(T), id);
    }

    public async Task<Guid> Create(T obj)
    {
        await _dbContext.Add<T>(obj);
        await _dbContext.SaveChangesAsync();

        return obj.Id;
    }

    public async Task<Guid> Update(T entity, params Expression<Func<T, object>>[] expressions)
    {
        var entry = _dbContext.Entry(entity);
        foreach (var expression in expressions)
        {
            entry.Property(expression).IsModified = true;
        }
        
        await _dbContext.SaveChangesAsync();
        
        return entity.Id;
    }

    public async Task<Guid> Delete(Guid id)
    {
        _dbContext.Remove<T>(id);
        await _dbContext.SaveChangesAsync();

        return id;
    }

    public async Task CreateRange(List<T> objs)
    { 
        _dbContext.AddRange<T>(objs);
        await _dbContext.SaveChangesAsync();
    }
}