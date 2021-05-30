using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebLicense.Core.Models.Customers;

namespace WebLicense.Access.Configuration
{
    public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasOne(q => q.Settings).WithOne().HasForeignKey<CustomerSettings>(q => q.CustomerId).IsRequired();
        }
    }
}