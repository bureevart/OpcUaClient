using System.Collections.Concurrent;
using OpcUaClient.Domain.Interfaces.Services;
using OpcUaClient.Domain.Models;
using OpcUaClient.Services.Interfaces;
using OpcUaClient.Services.OpcUa.OpcUaWorker;

namespace OpcUaClient.Services.OpcUa;

/// <summary>
/// Глобальный реестр всех <see cref="OpcUaWorker"/>-ов приложения.  
/// Создаётся как <strong>Singleton</strong> и предоставляет CRUD-API для серверов и тегов.
/// </summary>

public sealed class OpcUaHubService(
    IServiceScopeFactory scopeFactory,
    IConfiguration cfg,
    ITagCacheService tagCache)
    : IOpcUaHub, IDisposable
{
    private readonly ConcurrentDictionary<string, OpcUaWorker.OpcUaWorker> _workers = new();

    /* ────────── helpers ────────── */
    private static string Key(string ip, string port) => $"{ip}:{port}";
    private static (string ip, string port) Split(string key)
    {
        var parts = key.Split(':');
        return (parts[0], parts[1]);
    }

    /* ────────── Server CRUD ────────── */
    public bool CreateServer(string ip, string port)
    {
        var key = Key(ip, port);
        return _workers.TryAdd(key,
            new OpcUaWorker.OpcUaWorker(ip, port, cfg, scopeFactory, tagCache));
    }

    public bool RemoveServer(string ip, string port)
    {
        var key = Key(ip, port);
        if (_workers.TryRemove(key, out var worker))
        {
            worker.Dispose();
            return true;
        }
        return false;
    }

    /* ────────── Tag management ────────── */
    public bool AddTag(string ip, string port, Tag tag)
    {
        return TryGetWorker(ip, port, out var w) && w!.Active && ExecuteSafe(() => { w.AddMonitoredItem(tag); return true; });
    }

    public bool RemoveTag(string ip, string port, string displayName)
    {
        return TryGetWorker(ip, port, out var w) && w!.Active && ExecuteSafe(() => w.RemoveMonitoredItem(displayName));
    }

    /* ────────── Query helpers ────────── */
    public IReadOnlyCollection<string> ListServers() => _workers.Keys.ToList();

    public bool TryGetWorker(string ip, string port, out OpcUaWorker.OpcUaWorker? worker)
        => _workers.TryGetValue(Key(ip, port), out worker);

    /* ────────── Utils ────────── */
    private static bool ExecuteSafe(Func<bool> action)
    {
        try { return action(); }
        catch { return false; }
    }

    #region IDisposable
    public void Dispose()
    {
        foreach (var (_, worker) in _workers)
            worker.Dispose();
        _workers.Clear();
    }
    #endregion
}
