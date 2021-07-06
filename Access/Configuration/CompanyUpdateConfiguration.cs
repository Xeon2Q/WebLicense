using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text.Json;
using WebLicense.Core.Models.Auxiliary;
using WebLicense.Core.Models.Companies;

namespace WebLicense.Access.Configuration
{
    public sealed class CompanyUpdateConfiguration : IEntityTypeConfiguration<CompanyUpdate>
    {
        public void Configure(EntityTypeBuilder<CompanyUpdate> builder)
        {
            builder.HasOne(q => q.Company).WithMany(q => q.Updates).HasForeignKey(q => q.CompanyId).IsRequired();
            builder.HasOne(q => q.User).WithMany().HasForeignKey(q => q.UserId).IsRequired();

            builder.Property(q => q.Changes)
                   .HasConversion(
                       q => JsonSerializer.Serialize(q, null, new() {WriteIndented = false, IgnoreNullValues = true}),
                       q => JsonSerializer.Deserialize<ICollection<ValueUpdateInfo>>(q, new() {WriteIndented = false, IgnoreNullValues = true}))
                   .HasMaxLength(int.MaxValue);
        }
    }
}