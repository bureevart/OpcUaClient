namespace OpcUaClient.DataAccessLayer.Utils;

public class ConfigurationUtils
{
    public static string RemoveSuffix(
        string suffixToDelete,
        string tableName) 
    {
        if (string.IsNullOrEmpty(tableName) || !tableName.EndsWith(suffixToDelete))
            return tableName;

        return tableName[..^suffixToDelete.Length];
    }
}