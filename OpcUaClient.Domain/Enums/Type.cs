namespace OpcUaClient.Domain.Enums;

public enum Type
{
    None = 0,
    Bool = 1,
    Int16 = 2,
    Uint16 = 3,
    Int32 = 4,
    Uint32 = 5,
    Float = 6,
    Double = 7,
    String = 8,
    DateTime = 9
}

public static class TypeExtensions
{
    public static int GetRegistersCountByType(this Type type)
    {
        var bytes = -1;
        switch (type)
        {
            case Type.Bool:
            case Type.Int16:
            case Type.Uint16:
                bytes = 1;
                break;
            case Type.Float:
            case Type.Int32:
            case Type.Uint32:
                bytes = 2;
                break;
            case Type.Double:
            case Type.DateTime:
                bytes = 4;
                break;
            case Type.String:
            default: 
                break;                        
        }

        return bytes;
    }
}