using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpcUaClient.DataAccessLayer.Utils;
using OpcUaClient.Domain.Models;

namespace OpcUaClient.DataAccessLayer.EntityConfigurations;

public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable(ConfigurationUtils.RemoveSuffix(Constants.EntityPrefix, nameof(Tag)));
        builder.HasKey(tag => tag.Id);
    }
}