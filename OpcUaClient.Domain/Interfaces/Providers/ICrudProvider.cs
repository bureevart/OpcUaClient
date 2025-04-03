using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace OpcUaClient.Domain.Interfaces.Providers;

public interface ICrudProvider<T>
{
    public Task<List<T>> GetAll(Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool asNoTracking = false);
    public Task<T> Get(Guid id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? includes = null, bool asNoTracking = false);
    public Task<Guid> Create(T obj);
    public Task<Guid> Update(T entity, params Expression<Func<T, object>>[] expressions);
    public Task<Guid> Delete(Guid id);
    public Task CreateRange(List<T> objs);
}