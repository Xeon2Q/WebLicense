using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Access.Configuration
{
    public sealed class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            Seed(builder);
        }

        private void Seed(EntityTypeBuilder<Role> builder)
        {
            var roles = new Role[]
            {
                new() {Id = Roles.AdminId, Name = Roles.Admin, NormalizedName = Roles.AdminNormalized},
                new() {Id = Roles.CustomerAdminId, Name = Roles.CustomerAdmin, NormalizedName = Roles.CustomerAdminNormalized},
                new() {Id = Roles.CustomerManagerId, Name = Roles.CustomerManager, NormalizedName = Roles.CustomerManagerNormalized},
                new() {Id = Roles.CustomerUserId, Name = Roles.CustomerUser, NormalizedName = Roles.CustomerUserNormalized}
            };

            builder.HasData(roles);
        }
    }
}