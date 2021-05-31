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
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.OwnAccount.CanChangePassword.ClaimType, ClaimValue = WLClaims.OwnAccount.CanChangePassword.Claim.Value},
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.OwnAccount.CanDisable2FA.ClaimType, ClaimValue = WLClaims.OwnAccount.CanDisable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.OwnAccount.CanEnable2FA.ClaimType, ClaimValue = WLClaims.OwnAccount.CanEnable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.OwnAccount.CanLoginExternal.ClaimType, ClaimValue = WLClaims.OwnAccount.CanLoginExternal.Claim.Value},
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.OwnAccount.CanResetPassword.ClaimType, ClaimValue = WLClaims.OwnAccount.CanResetPassword.Claim.Value},

                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.Administration.Account.CanChangePassword.ClaimType, ClaimValue = WLClaims.Administration.Account.CanChangePassword.Claim.Value},
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.Administration.Account.CanDisable2FA.ClaimType, ClaimValue = WLClaims.Administration.Account.CanDisable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.AdminId, ClaimType = WLClaims.Administration.Account.CanEnable2FA.ClaimType, ClaimValue = WLClaims.Administration.Account.CanEnable2FA.Claim.Value},
                new(){Id = id, RoleId = Roles.AdminId, ClaimType = WLClaims.Administration.Account.CanResetPassword.ClaimType, ClaimValue = WLClaims.Administration.Account.CanResetPassword.Claim.Value}
            };
            builder.HasData(claims);
        }

        private static void SeenCustomerAdminClaims(EntityTypeBuilder<RoleClaim> builder)
        {
            var id = int.MaxValue - 100;

            var claims = new RoleClaim[]
            {
                new(){Id = id--, RoleId = Roles.CustomerAdminId, ClaimType = WLClaims.OwnAccount.CanChangePassword.ClaimType, ClaimValue = WLClaims.OwnAccount.CanChangePassword.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerAdminId, ClaimType = WLClaims.OwnAccount.CanDisable2FA.ClaimType, ClaimValue = WLClaims.OwnAccount.CanDisable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerAdminId, ClaimType = WLClaims.OwnAccount.CanEnable2FA.ClaimType, ClaimValue = WLClaims.OwnAccount.CanEnable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerAdminId, ClaimType = WLClaims.OwnAccount.CanLoginExternal.ClaimType, ClaimValue = WLClaims.OwnAccount.CanLoginExternal.Claim.Value},
                new(){Id = id, RoleId = Roles.CustomerAdminId, ClaimType = WLClaims.OwnAccount.CanResetPassword.ClaimType, ClaimValue = WLClaims.OwnAccount.CanResetPassword.Claim.Value}
            };
            builder.HasData(claims);
        }

        private static void SeenCustomerManagerClaims(EntityTypeBuilder<RoleClaim> builder)
        {
            var id = int.MaxValue - 200;

            var claims = new RoleClaim[]
            {
                new(){Id = id--, RoleId = Roles.CustomerManagerId, ClaimType = WLClaims.OwnAccount.CanChangePassword.ClaimType, ClaimValue = WLClaims.OwnAccount.CanChangePassword.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerManagerId, ClaimType = WLClaims.OwnAccount.CanDisable2FA.ClaimType, ClaimValue = WLClaims.OwnAccount.CanDisable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerManagerId, ClaimType = WLClaims.OwnAccount.CanEnable2FA.ClaimType, ClaimValue = WLClaims.OwnAccount.CanEnable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerManagerId, ClaimType = WLClaims.OwnAccount.CanLoginExternal.ClaimType, ClaimValue = WLClaims.OwnAccount.CanLoginExternal.Claim.Value},
                new(){Id = id, RoleId = Roles.CustomerManagerId, ClaimType = WLClaims.OwnAccount.CanResetPassword.ClaimType, ClaimValue = WLClaims.OwnAccount.CanResetPassword.Claim.Value}
            };
            builder.HasData(claims);
        }

        private static void SeenCustomerUserClaims(EntityTypeBuilder<RoleClaim> builder)
        {
            var id = int.MaxValue - 300;

            var claims = new RoleClaim[]
            {
                new(){Id = id--, RoleId = Roles.CustomerUserId, ClaimType = WLClaims.OwnAccount.CanChangePassword.ClaimType, ClaimValue = WLClaims.OwnAccount.CanChangePassword.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerUserId, ClaimType = WLClaims.OwnAccount.CanDisable2FA.ClaimType, ClaimValue = WLClaims.OwnAccount.CanDisable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerUserId, ClaimType = WLClaims.OwnAccount.CanEnable2FA.ClaimType, ClaimValue = WLClaims.OwnAccount.CanEnable2FA.Claim.Value},
                new(){Id = id--, RoleId = Roles.CustomerUserId, ClaimType = WLClaims.OwnAccount.CanLoginExternal.ClaimType, ClaimValue = WLClaims.OwnAccount.CanLoginExternal.Claim.Value},
                new(){Id = id, RoleId = Roles.CustomerUserId, ClaimType = WLClaims.OwnAccount.CanResetPassword.ClaimType, ClaimValue = WLClaims.OwnAccount.CanResetPassword.Claim.Value}
            };
            builder.HasData(claims);
        }

        #endregion
    }
}