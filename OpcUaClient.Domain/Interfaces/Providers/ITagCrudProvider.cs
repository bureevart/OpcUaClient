using OpcUaClient.Domain.Models;

namespace OpcUaClient.Domain.Interfaces.Providers;

public interface ITagCrudProvider : ICrudProvider<Tag>
{
    public Task<List<Tag>> GetRange(int offset, int limit);
    public Task<Tag> GetValue(Guid id);
}