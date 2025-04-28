using OpcUaClient.Domain.Models;

namespace OpcUaClient.Services.Interfaces;

public interface IOpcUaService
{
    public void AddMonitoringItem(Tag tag);
}