using Microsoft.EntityFrameworkCore;
using CloudObjects.Models;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CloudObjects.App.Data.Configurations
{
    public class AccountConfiguration : IEntityTypeConfiguration<Account>
    {
        public void Configure(EntityTypeBuilder<Account> builder)
        {
            builder.HasIndex(e => e.Name);
        }
    }
}
