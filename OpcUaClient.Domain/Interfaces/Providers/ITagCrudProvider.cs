using OpcUaClient.Domain.Models;

namespace OpcUaClient.Domain.Interfaces.Providers;

public interface ITagCrudProvider : ICrudProvider<Tag>
{
    public Task<Guid> Activate(Guid id);
    public Task<Guid> Deactivate(Guid id);
    public Task<string> GetValue(Guid id, bool logResult = false);
    public Task<Guid> SetAndWriteValue(Guid id);
    public Task<List<Tag>> GetTagsAsync(List<Guid> ids);
    public Task<List<Tag>> GetByGroupQueryId(Guid groupQueryId);
    public Task<string> GetConnectionString(Guid id);
    public Task<List<Tag>> GetRange(int offset, int limit);
}