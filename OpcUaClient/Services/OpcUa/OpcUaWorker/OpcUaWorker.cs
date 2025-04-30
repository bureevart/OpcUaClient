using MassTransit;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using OpcUaClient.Domain.Interfaces.Providers;
using OpcUaClient.Domain.Interfaces.Services;
using OpcUaClient.Domain.Models;
using SharedModels;

namespace OpcUaClient.Services.OpcUa.OpcUaWorker;

/// <summary>
/// «Самодостаточный» воркер, обслуживающий ОДИН OPC UA-сервер (IP + Port).  
/// Создаёт/рвёт <see cref="Session"/>, держит <see cref="Subscription"/>,
/// умеет добавлять новые теги и пересоздавать сессию при обрыве.
/// </summary>
public sealed class OpcUaWorker : IDisposable
{
    public string Key { get; } // "ip:port"
    public bool Active => _subscription?.PublishingEnabled ?? false;
    public DateTime LastSessionRenew { get; private set; }
    public DateTime LastServerAlive { get; private set; }

    private readonly IConfiguration _cfg;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ITagCacheService _tagCache;

    private Session? _session;
    private Subscription? _subscription;
    private readonly string _namespace;
    private readonly bool _useSecurity   = false;
    private readonly bool _autoRenew     = true;
    private readonly double _renewPeriodM = 60;

    private readonly Thread _renewerThread;
    private volatile bool _disposing;

    #region ctor / init
    public OpcUaWorker(
        string ip,
        string port,
        IConfiguration cfg,
        IServiceScopeFactory scopeFactory,
        ITagCacheService tagCache)
    {
        Key = $"{ip}:{port}";
        _cfg = cfg;
        _scopeFactory = scopeFactory;
        _tagCache = tagCache;

        _namespace = cfg["OpcUaSettings:OpcNameSpace"] ?? "2";

        InitializeOpcUaClient(ip, port);

        if (_autoRenew)
        {
            LastSessionRenew = DateTime.Now;
            _renewerThread   = new Thread(RenewLoop) { IsBackground = true };
            _renewerThread.Start();
        }
    }

    /// <summary>
    /// Создаёт сессию и базовую подписку.
    /// </summary>
    /// <param name="ip">server ip</param>
    /// <param name="port">server port</param>
    private void InitializeOpcUaClient(string ip, string port)
    {
        /* 1. Application configuration & certs */
        var appConfig = new ApplicationConfiguration
        {
            ApplicationName = nameof(OpcUaClient),
            ApplicationUri = Utils.Format(@"urn:{0}:" + nameof(OpcUaClient), ip),
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier()
                /* при необходимости заполните хранилища сертификатов */
            },
            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
            TraceConfiguration = new TraceConfiguration()
        };
        appConfig.Validate(ApplicationType.Client).GetAwaiter().GetResult();

        var app = new ApplicationInstance
        {
            ApplicationName = nameof(OpcUaClient),
            ApplicationType = ApplicationType.Client,
            ApplicationConfiguration = appConfig
        };
        app.CheckApplicationInstanceCertificate(false, 2048).GetAwaiter().GetResult();

        /* 2. Endpoint & Session */
        var ep = CoreClientUtils.SelectEndpoint($"opc.tcp://{ip}:{port}", _useSecurity);
        _session = Session.Create(
            appConfig,
            new ConfiguredEndpoint(null, ep, EndpointConfiguration.Create(appConfig)),
            updateBeforeConnect: false,
            sessionName: "",
            sessionTimeout: 60_000,
            identity: null,
            preferredLocales: null).GetAwaiter().GetResult();

        /* 3. Base subscription (heartbeat) */
        _subscription = new Subscription(_session.DefaultSubscription) { PublishingInterval = 1_000 };

        var heartbeat = new MonitoredItem(_subscription.DefaultItem)
        {
            DisplayName = "ServerStatusCurrentTime",
            StartNodeId = "i=2258"
        };
        heartbeat.Notification += OnTagValueChange;

        _subscription.AddItem(heartbeat);
        _session.AddSubscription(_subscription);
        _subscription.Create();

        _subscription.PublishingEnabled = true;
    }
    #endregion

    #region public API
    /// <summary>Добавляет тег в подписку (если ещё не добавлен).</summary>
    public void AddMonitoredItem(Tag tag)
    {
        if (_subscription == null)
            throw new InvalidOperationException("Subscription not created");

        if (_tagCache.GetTag(tag.DisplayName) != null)
            return;

        var item = new MonitoredItem(_subscription.DefaultItem)
        {
            DisplayName = tag.DisplayName,
            StartNodeId = $"ns={_namespace};i={tag.NodeId}"
        };
        item.Notification += OnTagValueChange;

        _subscription.AddItem(item);
        _subscription.ApplyChanges();
        _tagCache.SetTag(tag.DisplayName, tag);
    }
    
    /// <summary>Удаляет тег из подписки.</summary>
    public bool RemoveMonitoredItem(string displayName)
    {
        if (_subscription == null) return false;

        var item = _subscription.MonitoredItems
            .FirstOrDefault(m => m.DisplayName == displayName);
        if (item == null) return false;

        _subscription.RemoveItem(item);
        _subscription.ApplyChanges();
        _tagCache.DeleteTag(displayName);
        return true;
    }

    /// <summary>Приостановить публикацию (не рвёт соединение).</summary>
    public void Stop() => _subscription!.PublishingEnabled = false;

    /// <summary>Возобновить публикацию, если была остановлена.</summary>
    public void Start() => _subscription!.PublishingEnabled = true;
    #endregion

    #region Renew loop & Callback
    private void RenewLoop()
    {
        while (!_disposing)
        {
            try
            {
                if ((_session == null || !_session.Connected) ||
                    (DateTime.Now - LastSessionRenew).TotalMinutes > _renewPeriodM ||
                    (DateTime.Now - LastServerAlive).TotalSeconds  > 60)
                {
                    RecreateSession().GetAwaiter().GetResult(); //TODO
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RenewLoop] {Key}: {ex}");
            }

            Thread.Sleep(TimeSpan.FromSeconds(2));
        }
    }

    private async Task RecreateSession()
    {
        Console.WriteLine($"[{Key}] Recreating session…");

        _subscription?.DeleteAsync(true);
        _session?.CloseAsync();
        _session?.Dispose();

        var (ip, port) = Key.Split(':') switch { var s => (s[0], s[1]) };
        InitializeOpcUaClient(ip, port);
        LastSessionRenew = DateTime.Now;

        /* перерегистрируем теги из кэша */
        using var scope = _scopeFactory.CreateScope();
        var tagCrudProvider = scope.ServiceProvider.GetRequiredService<ITagCrudProvider>();
        var tags = await tagCrudProvider.GetAll();
        
        foreach (var tag in tags)
            AddMonitoredItem(tag);
    }

    private void OnTagValueChange(MonitoredItem item, MonitoredItemNotificationEventArgs e)
    {
        foreach (var value in item.DequeueValues())
        {
            if (item.DisplayName == "ServerStatusCurrentTime")
            {
                LastServerAlive = value.SourceTimestamp.ToLocalTime();
                continue;
            }

            // === логика обработки ваших тегов ===
            var tag = _tagCache.GetTag(item.DisplayName);
            if (tag != null)
            {
                if (value.Value != null)
                {
                    tag.LastGoodValue = value.Value.ToString();
                    tag.CurrentValue = value.Value.ToString();
                    tag.LastUpdatedTime = DateTime.Now;
                    tag.LastSourceTimeStamp = value.SourceTimestamp.ToLocalTime();
                    tag.StatusCode = value.StatusCode.ToString();
                }
                else
                {
                    tag.StatusCode = value.StatusCode.ToString();
                    tag.CurrentValue = null;
                }
                _tagCache.SetTag(tag.DisplayName, tag);
            }

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var pub = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                pub.Publish(new TagModel
                {
                    NodeId = item.StartNodeId.ToString(),
                    DisplayName = item.DisplayName,
                    Value = value.Value?.ToString()
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Publish] {Key}: {ex}");
            }
        }
    }
    #endregion

    #region IDisposable
    public void Dispose()
    {
        _disposing = true;

        try
        {
            _renewerThread.Join();
        }
        catch
        {
            // ignored
        }
        try   { _subscription?.Delete(true); }
        catch
        {
            // ignored
        }

        try   { _session?.Close(); }
        catch
        {
            // ignored
        }

        _session?.Dispose();
    }
    #endregion
}
