using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpcUaClient.DataAccessLayer.Utils;
using OpcUaClient.Domain.Models;

namespace OpcUaClient.DataAccessLayer.EntityConfigurations;

public class ServerConfiguration : IEntityTypeConfiguration<Server>
{
    public void Configure(EntityTypeBuilder<Server> builder)
    {
        builder.ToTable(ConfigurationUtils.RemoveSuffix(Constants.EntityPrefix, nameof(Server)));
        builder.HasKey(server => server.Id);
        builder.HasMany(server => server.Tags).WithOne(tag => tag.Server).HasForeignKey(tag => tag.ServerId);

    }
}