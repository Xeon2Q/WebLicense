using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebLicense.Core.Models.Auxiliary;
using WebLicense.Core.Models.Customers;

namespace WebLicense.Access.Configuration
{
    public sealed class CustomerUpdateConfiguration : IEntityTypeConfiguration<CustomerUpdate>
    {
        public void Configure(EntityTypeBuilder<CustomerUpdate> builder)
        {
            builder.HasOne(q => q.User).WithMany().HasForeignKey(q => q.UserId).IsRequired();

            builder.Property(q => q.Changes)
                   .HasConversion(
                       q => JsonSerializer.Serialize(q, null),
                       q => JsonSerializer.Deserialize<ICollection<ValueUpdateInfo>>(q, null))
                   .HasMaxLength(int.MaxValue);
        }
    }
}