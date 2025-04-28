using Type = OpcUaClient.Domain.Enums.Type;

namespace OpcUaClient.Models.TagModels;

public class TagCreateModel
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public DateTime LastUpdatedTime { get; set; }

    public DateTime LastSourceTimeStamp { get; set; }


    public string StatusCode { get; set; }

    public string? LastGoodValue { get; set; }
    public string? CurrentValue { get; set; }
    public string NodeId { get; set; }

    public string DisplayName { get; set; }
}