using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Access.Configuration
{
    public sealed class UserConfiguration : IEntityTypeConfiguration<User>
    {
        private const string InitialAdminPassword = "web-License?97Y13";

        public void Configure(EntityTypeBuilder<User> builder)
        {
            Seed(builder);
        }

        private void Seed(EntityTypeBuilder<User> builder)
        {
            var admin = new User
            {
                Id = long.MaxValue,
                UserName = "Administrator",
                NormalizedUserName = "Administrator".ToUpper(),
                Email = "admin-one@weblicense.com",
                NormalizedEmail = "admin-one@weblicense.com".ToUpper(),
                EmailConfirmed = true,
                PhoneNumber = null,
                PhoneNumberConfirmed = true,
                LockoutEnd = null,
                LockoutEnabled = false,
                EulaAccepted = true,
                GdprAccepted = true,
                SecurityStamp = $"{Guid.NewGuid():N}{Guid.NewGuid():N}{Guid.NewGuid():N}".ToUpper(),
                TwoFactorEnabled = false
            };

            admin.PasswordHash = new PasswordHasher<User>().HashPassword(admin, InitialAdminPassword);

            builder.HasData(admin);
        }
    }
}