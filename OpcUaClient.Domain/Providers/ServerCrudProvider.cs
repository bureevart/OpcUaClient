using OpcUaClient.Domain.Exceptions;
using OpcUaClient.Domain.Interfaces;
using OpcUaClient.Domain.Interfaces.Providers;
using OpcUaClient.Domain.Models;

namespace OpcUaClient.Domain.Providers;

public class ServerCrudProvider(IServiceDbContext context) : BaseCrudProvider<Server>(context), IServerCrudProvider
{
    public async Task<long> Activate(Guid id)
    {
        var el = _dbContext.Find<Server>(id);
        if (el == null) throw new EntityNotFoundException(typeof(Server), id);

        if (!el.Active)
        {
            el.Active = true;
            await _dbContext.SaveChangesAsync();
            return 1;
        }

        return 0;
    }

    public async Task<long> Deactivate(Guid id)
    {
        var el = _dbContext.Find<Server>(id);
        if (el == null) throw new EntityNotFoundException(typeof(Server), id);

        if (el.Active)
        {
            el.Active = false;
            await _dbContext.SaveChangesAsync();
            return 1;
        }

        return 0;
    }
}