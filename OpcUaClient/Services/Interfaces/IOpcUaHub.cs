using OpcUaClient.Domain.Models;
using OpcUaClient.Services.OpcUa.OpcUaWorker;

namespace OpcUaClient.Services.Interfaces;

public interface IOpcUaHub
{
    bool CreateServer(string ip, string port);
    bool RemoveServer(string ip, string port);

    bool AddTag(string ip, string port, Tag tag);
    bool RemoveTag(string ip, string port, string displayName);

    IReadOnlyCollection<string> ListServers();
    bool TryGetWorker(string ip, string port, out OpcUaWorker? worker);
}