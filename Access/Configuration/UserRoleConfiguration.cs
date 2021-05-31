using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Access.Configuration
{
    public sealed class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            Seed(builder);
        }

        private void Seed(EntityTypeBuilder<UserRole> builder)
        {
            var adminRole = new UserRole {RoleId = Roles.AdminId, UserId = long.MaxValue};

            builder.HasData(adminRole);
        }
    }
}