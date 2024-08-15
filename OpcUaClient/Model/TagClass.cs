using System;
using Opc.Ua.Client;

namespace OpcUaClient.Model;

public class TagClass
{

    public TagClass(string displayName, string nodeID)
    {
        DisplayName = displayName;
        NodeID = nodeID;

    }

    public DateTime LastUpdatedTime { get; set; }

    public DateTime LastSourceTimeStamp { get; set; }


    public string StatusCode { get; set; }

    public string? LastGoodValue { get; set; }
    public string? CurrentValue { get; set; }
    public string NodeID { get; set; }

    public string DisplayName { get; set; }
}