using Microsoft.EntityFrameworkCore;
using OpcUaClient.Domain.Interfaces;
using OpcUaClient.Domain.Interfaces.Providers;
using OpcUaClient.Domain.Interfaces.Services;
using OpcUaClient.Domain.Models;

namespace OpcUaClient.Domain.Providers;

public class TagCrudProvider(IServiceDbContext context, ITagCacheService tagCacheService) : BaseCrudProvider<Tag>(context), ITagCrudProvider
{
    public Task<List<Tag>> GetRange(int offset, int limit)
    {
        return _dbContext.Set<Tag>()
            .AsQueryable()
            .AsNoTracking()
            .Skip(offset)
            .Take(limit)
            .ToListAsync();
    }

    public async Task<Tag> GetValue(Guid id)
    {
        var tag = await Get(id);

        return tagCacheService.GetTag(tag.DisplayName) ?? throw new InvalidOperationException();
    }
}