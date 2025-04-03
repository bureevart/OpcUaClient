using Microsoft.EntityFrameworkCore;
using OpcUaClient.Domain.Exceptions;
using OpcUaClient.Domain.Interfaces;
using OpcUaClient.Domain.Interfaces.Providers;
using OpcUaClient.Domain.Models;

namespace OpcUaClient.Domain.Providers;

public class TagCrudProvider(IServiceDbContext context) : BaseCrudProvider<Tag>(context), ITagCrudProvider
{
    public async Task<Guid> Activate(Guid id)
    {
        var el = _dbContext.Find<Tag>(id);
        if (el == null) throw new EntityNotFoundException(typeof(Tag), id);

        el.Active = true;
        
        await _dbContext.SaveChangesAsync();

        return el.Id;
    }

    public async Task<Guid> Deactivate(Guid id)
    {
        var el = _dbContext.Find<Tag>(id);
        if (el == null) throw new EntityNotFoundException(typeof(Tag), id);

        el.Active = false;
        
        await _dbContext.SaveChangesAsync();

        return el.Id;
    }

    public Task<string> GetValue(Guid id, bool logResult = false)
    {
        throw new NotImplementedException();
    }

    public Task<Guid> SetAndWriteValue(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Tag>> GetTagsAsync(List<Guid> ids)
    {
        throw new NotImplementedException();
    }

    public Task<List<Tag>> GetByGroupQueryId(Guid groupQueryId)
    {
        throw new NotImplementedException();
    }

    public Task<string> GetConnectionString(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Tag>> GetRange(int offset, int limit)
    {
        return _dbContext.Set<Tag>()
            .AsQueryable()
            .AsNoTracking()
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }
}