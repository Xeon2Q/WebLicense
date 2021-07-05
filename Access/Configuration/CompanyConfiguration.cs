using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebLicense.Core.Models.Companies;
using WebLicense.Core.Models.Customers;

namespace WebLicense.Access.Configuration
{
    public sealed class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasMany(q => q.CompanyCustomers).WithOne(q => q.Company).HasForeignKey(q => q.CompanyId).IsRequired();

            builder.HasMany(q => q.Users)
                   .WithMany(q => q.Companies)
                   .UsingEntity<CompanyUser>(
                       q => q.HasOne(w => w.User).WithMany(w => w.CompanyUsers).HasForeignKey(w => w.UserId).IsRequired(),
                       q => q.HasOne(w => w.Company).WithMany(w => w.CompanyUsers).HasForeignKey(w => w.CompanyId).IsRequired(),
                       q => q.HasKey(w => new {w.CompanyId, w.UserId}));

            builder.HasMany(q => q.Customers)
                   .WithMany(q => q.Companies)
                   .UsingEntity<CustomerSettings>(
                       q => q.HasOne(w => w.Customer).WithMany(w => w.Settings).HasForeignKey(w => w.CustomerId).IsRequired(),
                       q => q.HasOne(w => w.Company).WithMany(w => w.CompanyCustomers).HasForeignKey(w => w.CompanyId).IsRequired(),
                       q => q.HasKey(w => new {w.CompanyId, w.CustomerId}));
        }
    }
}