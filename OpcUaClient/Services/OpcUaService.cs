using MassTransit;
using Opc.Ua;
using Opc.Ua.Client;
using Opc.Ua.Configuration;
using OpcUaClient.Domain.Interfaces.Services;
using OpcUaClient.Domain.Models;
using OpcUaClient.Services.Cache;
using OpcUaClient.Services.Interfaces;
using SharedModels;

namespace OpcUaClient.Services;

public class OpcUaService : IOpcUaService
{
    public string ServerAddress { get; set; }
    public string ServerPortNumber { get; set; }
    public bool SecurityEnabled { get; set; } = false;
    public string MyApplicationName { get; set; } = nameof(OpcUaClient);
    public Session? Session { get; set; }
    public string OpcNameSpace { get; set; }
    private ITagCacheService _tagCacheService { get; set; }
    public bool SessionRenewalRequired { get; set; }
    public DateTime LastTimeSessionRenewed { get; set; }
    public DateTime LastTimeOpcServerFoundAlive { get; set; }
    public bool ClassDisposing { get; set; }
    
    private Thread _renewerThread;
    private double _sessionRenewalPeriodMins = 60;
    
    private readonly IServiceScopeFactory _serviceScopeFactory;
    
    public OpcUaService(IConfiguration configuration, IServiceScopeFactory serviceScopeFactory, ITagCacheService tagCacheService)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _tagCacheService = tagCacheService;
        
        ServerAddress = configuration.GetSection("OpcUaSettings")["Host"] ?? string.Empty;
        ServerPortNumber = configuration.GetSection("OpcUaSettings")["Port"] ?? string.Empty;
        OpcNameSpace = configuration.GetSection("OpcUaSettings")["OpcNameSpace"] ?? string.Empty;
        SessionRenewalRequired = configuration.GetSection("OpcUaSettings").GetValue<bool>("SessionRenewalRequired");
        
        InitializeOpcUaClient();
        
        if (SessionRenewalRequired)
        {
            LastTimeSessionRenewed = DateTime.Now;
            _renewerThread = new Thread(RenewSessionThread);
            _renewerThread.Start();
        }
    }
    
    private void InitializeOpcUaClient()
    {
        //Console.WriteLine("Step 1 - Create application configuration and certificate.");
        var config = new ApplicationConfiguration()
        {
            ApplicationName = MyApplicationName,
            ApplicationUri = Utils.Format(@"urn:{0}:" + MyApplicationName + "", ServerAddress),
            ApplicationType = ApplicationType.Client,
            SecurityConfiguration = new SecurityConfiguration
            {
                ApplicationCertificate = new CertificateIdentifier {/* StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\MachineDefault", SubjectName = Utils.Format(@"CN={0}, DC={1}", MyApplicationName, ServerAddress) */},
                /*TrustedIssuerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Certificate Authorities" },
                TrustedPeerCertificates = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\UA Applications" },
                RejectedCertificateStore = new CertificateTrustList { StoreType = @"Directory", StorePath = @"%CommonApplicationData%\OPC Foundation\CertificateStores\RejectedCertificates" },
                AutoAcceptUntrustedCertificates = true,
                AddAppCertToTrustedStore = true*/
            },
            TransportConfigurations = new TransportConfigurationCollection(),
            TransportQuotas = new TransportQuotas { OperationTimeout = 15000 },
            ClientConfiguration = new ClientConfiguration { DefaultSessionTimeout = 60000 },
            TraceConfiguration = new TraceConfiguration()
        };
        
        config.Validate(ApplicationType.Client).GetAwaiter().GetResult();
        
        if (config.SecurityConfiguration.AutoAcceptUntrustedCertificates)
        {
            config.CertificateValidator.CertificateValidation += (_, e) => { e.Accept = (e.Error.StatusCode == Opc.Ua.StatusCodes.BadCertificateUntrusted); };
        }

        var application = new ApplicationInstance
        {
            ApplicationName = MyApplicationName,
            ApplicationType = ApplicationType.Client,
            ApplicationConfiguration = config
        };
        application.CheckApplicationInstanceCertificate(false, 2048).GetAwaiter().GetResult();

        var serverAddress = ServerAddress;
        var selectedEndpoint = CoreClientUtils.SelectEndpoint("opc.tcp://" + serverAddress + ":" + ServerPortNumber + "", useSecurity: SecurityEnabled);

        // Console.WriteLine($"Step 2 - Create a session with your server: {selectedEndpoint.EndpointUrl} ");
        Session = Session.Create(config, new ConfiguredEndpoint(null, selectedEndpoint, EndpointConfiguration.Create(config)), false, "", 60000, null, null).GetAwaiter().GetResult();
        {


            //Console.WriteLine("Step 4 - Create a subscription. Set a faster publishing interval if you wish.");
            var subscription = new Subscription(Session.DefaultSubscription) { PublishingInterval = 1000 };

            //Console.WriteLine("Step 5 - Add a list of items you wish to monitor to the subscription.");
            var list = new List<MonitoredItem>
            {
                new MonitoredItem(subscription.DefaultItem)
                    { DisplayName = "ServerStatusCurrentTime", StartNodeId = "i=2258" }
            };
            
            list.ForEach(i => i.Notification += OnTagValueChange);
            subscription.AddItems(list);

            //Console.WriteLine("Step 6 - Add the subscription to the session.");
            Session.AddSubscription(subscription);
            subscription.Create();
        }
    }

    public void AddMonitoringItem(Tag tag)
    {
        if(_tagCacheService.GetTag(tag.DisplayName) is not null) 
                return;
        var subscription = Session?.Subscriptions.FirstOrDefault();
        var item = new MonitoredItem(subscription?.DefaultItem)
            { 
                DisplayName = tag.DisplayName, 
                StartNodeId = "ns=" + OpcNameSpace + ";i=" + tag.NodeId
            };
        subscription?.AddItem(item);
        item.Notification += OnTagValueChange;
        
        subscription?.ApplyChanges();
        _tagCacheService.SetTag(tag.DisplayName, tag);
    }
    
    private void RenewSessionThread()
    {
        while (!ClassDisposing)
        {
            if ((DateTime.Now - LastTimeSessionRenewed).TotalMinutes > _sessionRenewalPeriodMins
                || (DateTime.Now - LastTimeOpcServerFoundAlive).TotalSeconds > 60)
            {
                Console.WriteLine("Renewing Session");
                try
                {
                    Session?.Close();
                    Session?.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                InitializeOpcUaClient();
                LastTimeSessionRenewed = DateTime.Now;

            }
            Thread.Sleep(2000);

        }

    }
    
    [Obsolete("Obsolete")]
    ~OpcUaService()
    {

        ClassDisposing = true;
        try
        {

            Session?.Close();
            Session?.Dispose();
            Session = null;
            _renewerThread.Abort();
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }

    }
    
    private void OnTagValueChange(MonitoredItem item, MonitoredItemNotificationEventArgs e)
    {

        foreach (var value in item.DequeueValues())
        {

            if (item.DisplayName == "ServerStatusCurrentTime")
            {
                LastTimeOpcServerFoundAlive = value.SourceTimestamp.ToLocalTime();
            }
            else
            {
                if (value.Value != null)
                    Console.WriteLine("{0}: {1}, {2}, {3}", item.DisplayName, value.Value, value.SourceTimestamp.ToLocalTime(), value.StatusCode);
                else
                    Console.WriteLine("{0}: {1}, {2}, {3}", item.DisplayName, "Null Value", value.SourceTimestamp, value.StatusCode);

                var tag = _tagCacheService.GetTag(item.DisplayName);
                
                if (tag is not null)
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
                    _tagCacheService.SetTag(tag.DisplayName, tag);
                }

                try
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var publishEndpoint = scope.ServiceProvider.GetRequiredService<IPublishEndpoint>();

                    publishEndpoint.Publish<TagModel>(new TagModel()
                    {
                        NodeId = item.StartNodeId.ToString(),
                        DisplayName = item.DisplayName,
                        Value = value.Value?.ToString()
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

        }
    }
}