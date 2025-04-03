using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using OpcUaClient.Domain.Interfaces;
using OpcUaClient.Domain.Models;

namespace OpcUaClient.DataAccessLayer;

public class ServiceDbContext(DbContextOptions<ServiceDbContext> options) : DbContext(options), IServiceDbContext
{
    public DbSet<Tag> Tags { get; set; }

    public string? GetConnectionString() => base.Database.GetConnectionString();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(),
            t => t.GetInterfaces().Any(i => i.IsGenericType &&
                                           i.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)));
        base.OnModelCreating(modelBuilder);
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging().EnableDetailedErrors();
        
        base.OnConfiguring(optionsBuilder);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await base.SaveChangesAsync();
    }
    
    public T? Find<T>(Guid id) where T : Entity
    {
        return base.Find<T>(id);
    }
    
    public DbSet<T> Set<T>() where T : Entity
    {
        return base.Set<T>();
    }
    
    public async Task<EntityEntry<T>> Add<T>(T entity) where T : Entity
    {
        var result = await base.AddAsync(entity);
        return result;
    }
    
    public void Remove<T>(Guid id) where T : Entity
    {
        var entity = base.Find<T>(id);
        base.Remove(entity);
    }
    
    public void Update<T>(T entity) where T : Entity
    {
        base.Update(entity);
    }
    public void UpdateRange<T>(T[] entities) where T : Entity
    {
        base.UpdateRange(entities);
    }

    public EntityEntry<T> Entry<T>(T entity) where T : Entity
    {
        return base.Entry(entity);
    }
    
    public void AttachRange<T>(T[] entities) where T : Entity
    {
        base.AttachRange(entities);
    }
    
    public EntityEntry<T> Attach<T>(T entity) where T : Entity
    {
        return base.Attach(entity);
    }
    
    public void Detach<T>(T entity) where T : class
    {
        var entry = base.Entry(entity);
        entry.State = EntityState.Detached;
    }

    public void ClearTrackingObjects()
    {
        base.ChangeTracker.Clear();
    }

    public void AddRange<T>(List<T> entities) where T : Entity
    {
        base.AddRange(entities);
    }
}