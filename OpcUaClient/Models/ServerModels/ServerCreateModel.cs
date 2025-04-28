namespace OpcUaClient.Models.ServerModels;

public class ServerCreateModel
{
    public Guid Id { get; set; }
    public string ApplicationName { get; set; } = nameof(OpcUaClient);
    public string ServerAddress { get; set; }
    public string ServerPortNumber { get; set; }
    public bool Active { get; set; }
}