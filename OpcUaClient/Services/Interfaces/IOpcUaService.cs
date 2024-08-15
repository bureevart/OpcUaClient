using OpcUaClient.Model;

namespace OpcUaClient.Services.Interfaces;

public interface IOpcUaService
{
    public void AddMonitoringItem(TagClass tag);
}