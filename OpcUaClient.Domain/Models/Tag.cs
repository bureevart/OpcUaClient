using Type = OpcUaClient.Domain.Enums.Type;


namespace OpcUaClient.Domain.Models;

public class Tag : Entity
{
    public Tag(string displayName, string nodeId)
    {
        DisplayName = displayName;
        NodeId = nodeId;

    }
    
    public Guid Id { get; set; }
    public string Name { get; set; }

    public DateTime LastUpdatedTime { get; set; }

    public DateTime LastSourceTimeStamp { get; set; }


    public string StatusCode { get; set; }

    public string? LastGoodValue { get; set; }
    public string? CurrentValue { get; set; }
    public string NodeId { get; set; }
    public string DisplayName { get; set; }
    public Server Server { get; set; }
    public Guid ServerId { get; set; }
}