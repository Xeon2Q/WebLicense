using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Shared.Auxiliary.Claims;

namespace WebLicense.Access.Configuration
{
    public sealed class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
    {
        public void Configure(EntityTypeBuilder<RoleClaim> builder)
        {
            Seed(builder);
        }

        private void Seed(EntityTypeBuilder<RoleClaim> builder)
        {
            SeenAdminClaims(builder);
            SeenCustomerAdminClaims(builder);
            SeenCustomerManagerClaims(builder);
            SeenCustomerUserClaims(builder);
        }

        #region Methods

        private static void SeenAdminClaims(EntityTypeBuilder<RoleClaim> builder)
        {
            var id = int.MaxValue;

            var claims = new RoleClaim[]
            {
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.Account.ChangePassword.ClaimType, ClaimValue = WLClaims.Account.ChangePassword.Claim.Value},
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.Account.Disable2FA.ClaimType, ClaimValue = WLClaims.Account.Disable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.Account.Enable2FA.ClaimType, ClaimValue = WLClaims.Account.Enable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.Account.LoginExternal.ClaimType, ClaimValue = WLClaims.Account.LoginExternal.Claim.Value},
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.Account.ResetPassword.ClaimType, ClaimValue = WLClaims.Account.ResetPassword.Claim.Value},

                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.Administration.Account.ChangePassword.ClaimType, ClaimValue = WLClaims.Administration.Account.ChangePassword.Claim.Value},
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.Administration.Account.Disable2FA.ClaimType, ClaimValue = WLClaims.Administration.Account.Disable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.Administration.Account.Enable2FA.ClaimType, ClaimValue = WLClaims.Administration.Account.Enable2FA.Claim.Value},
                new(){Id = id, RoleId = Roles.AdminId, ClaimType = WLClaims.Administration.Account.ResetPassword.ClaimType, ClaimValue = WLClaims.Administration.Account.ResetPassword.Claim.Value}
            };
            builder.HasData(claims);
        }

        private static void SeenCustomerAdminClaims(EntityTypeBuilder<RoleClaim> builder)
        {
            var id = int.MaxValue - 100;

            var claims = new RoleClaim[]
            {
                new(){Id = id--, RoleId = Roles.CustomerAdminId, ClaimType = WLClaims.Account.ChangePassword.ClaimType, ClaimValue = WLClaims.Account.ChangePassword.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerAdminId, ClaimType = WLClaims.Account.Disable2FA.ClaimType, ClaimValue = WLClaims.Account.Disable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerAdminId, ClaimType = WLClaims.Account.Enable2FA.ClaimType, ClaimValue = WLClaims.Account.Enable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerAdminId, ClaimType = WLClaims.Account.LoginExternal.ClaimType, ClaimValue = WLClaims.Account.LoginExternal.Claim.Value},
                new(){Id = id, RoleId = Roles.CustomerAdminId, ClaimType = WLClaims.Account.ResetPassword.ClaimType, ClaimValue = WLClaims.Account.ResetPassword.Claim.Value}
            };
            builder.HasData(claims);
        }

        private static void SeenCustomerManagerClaims(EntityTypeBuilder<RoleClaim> builder)
        {
            var id = int.MaxValue - 200;

            var claims = new RoleClaim[]
            {
                new(){Id = id--, RoleId = Roles.CustomerManagerId, ClaimType = WLClaims.Account.ChangePassword.ClaimType, ClaimValue = WLClaims.Account.ChangePassword.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerManagerId, ClaimType = WLClaims.Account.Disable2FA.ClaimType, ClaimValue = WLClaims.Account.Disable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerManagerId, ClaimType = WLClaims.Account.Enable2FA.ClaimType, ClaimValue = WLClaims.Account.Enable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerManagerId, ClaimType = WLClaims.Account.LoginExternal.ClaimType, ClaimValue = WLClaims.Account.LoginExternal.Claim.Value},
                new(){Id = id, RoleId = Roles.CustomerManagerId, ClaimType = WLClaims.Account.ResetPassword.ClaimType, ClaimValue = WLClaims.Account.ResetPassword.Claim.Value}
            };
            builder.HasData(claims);
        }

        private static void SeenCustomerUserClaims(EntityTypeBuilder<RoleClaim> builder)
        {
            var id = int.MaxValue - 300;

            var claims = new RoleClaim[]
            {
                new(){Id = id--, RoleId = Roles.CustomerUserId, ClaimType = WLClaims.Account.ChangePassword.ClaimType, ClaimValue = WLClaims.Account.ChangePassword.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerUserId, ClaimType = WLClaims.Account.Disable2FA.ClaimType, ClaimValue = WLClaims.Account.Disable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerUserId, ClaimType = WLClaims.Account.Enable2FA.ClaimType, ClaimValue = WLClaims.Account.Enable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerUserId, ClaimType = WLClaims.Account.LoginExternal.ClaimType, ClaimValue = WLClaims.Account.LoginExternal.Claim.Value},
                new(){Id = id, RoleId = Roles.CustomerUserId, ClaimType = WLClaims.Account.ResetPassword.ClaimType, ClaimValue = WLClaims.Account.ResetPassword.Claim.Value}
            };
            builder.HasData(claims);
        }

        #endregion
    }
}