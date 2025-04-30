using OpcUaClient.Domain.Models;

namespace OpcUaClient.Domain.Interfaces.Providers;

public interface IServerCrudProvider : ICrudProvider<Server>
{
    public Task<long> Activate(Guid id);
    public Task<long> Deactivate(Guid id);
}