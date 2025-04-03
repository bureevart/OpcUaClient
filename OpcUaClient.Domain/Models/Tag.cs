using Type = OpcUaClient.Domain.Enums.Type;


namespace OpcUaClient.Domain.Models;

public class Tag : Entity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public string Comment { get; set; } = "";
    public bool Active { get; set; }
    public int Address { get; set; }
    public Type Type { get; set; }
    public bool Recalc { get; set; }
    public float Factor { get; set; }
    public float Offset { get; set; }
    public string Value { get; set; } = "";
    public bool HasWriteRegister { get; set; }
    public int WriteRegisterAddress { get; set; }
    public Type OutputType { get; set; }
    public bool UseOutputType { get; set; }
    public byte RoundingAccuracy { get; set; } = 7;
    public bool ConvertBeforeRecalcForRead { get; set; } = true;
    public bool ConvertBeforeRecalcForWrite { get; set; } = false;
}