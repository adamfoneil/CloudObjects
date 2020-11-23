using Microsoft.EntityFrameworkCore;
using CloudObjects.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudObjects.App.Data.Configurations
{
    public class StoredObjectConfiguration : IEntityTypeConfiguration<StoredObject>
    {
        public void Configure(EntityTypeBuilder<StoredObject> builder)
        {
            builder.HasIndex(e => e.AccountId);
            builder.HasIndex(e => e.Name);
        }
    }
}
