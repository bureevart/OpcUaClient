using Microsoft.EntityFrameworkCore;

namespace OpcUaClient.DataAccessLayer;

public static class DbInitializer
{
    public static void Initialize(ServiceDbContext context)
    {
        context.Database.Migrate();
    }
}