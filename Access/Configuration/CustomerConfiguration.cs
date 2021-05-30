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

            builder.HasMany(q => q.Administrators)
                   .WithMany(q => q.AdministeredCustomers)
                   .UsingEntity<CustomerAdministrator>(
                       q => q.HasOne(w => w.User).WithMany(w => w.CustomerAdministrators).HasForeignKey(w => w.UserId),
                       q => q.HasOne(w => w.Customer).WithMany(w => w.CustomerAdministrators).HasForeignKey(w => w.CustomerId),
                       q => q.HasKey(w => new {w.CustomerId, w.UserId}));

            builder.HasMany(q => q.Managers)
                   .WithMany(q => q.ManagedCustomers)
                   .UsingEntity<CustomerManager>(
                       q => q.HasOne(w => w.User).WithMany(w => w.CustomerManagers).HasForeignKey(w => w.UserId),
                       q => q.HasOne(w => w.Customer).WithMany(w => w.CustomerManagers).HasForeignKey(w => w.CustomerId),
                       q => q.HasKey(w => new {w.CustomerId, w.UserId}));
        }
    }
}