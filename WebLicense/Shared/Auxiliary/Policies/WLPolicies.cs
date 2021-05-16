using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Authorization;
using WebLicense.Shared.Auxiliary.Claims;

namespace WebLicense.Shared.Auxiliary.Policies
{
    public static class WLPolicies
    {
        public static class OwnAccount
        {
            public static class Names
            {
                public const string CanLoginExternal = "https://weblicense/account/login/external";

                public const string CanResetPassword = "https://weblicense/account/password/reset";

                public const string CanChangePassword = "https://weblicense/account/password/change";

                public const string CanDisable2FA = "https://weblicense/account/2fa/disable";

                public const string CanEnable2FA = "https://weblicense/account/2fa/enable";
            }

            public static class Policies
            {
                public static WLPolicy CanLoginExternal => new(Names.CanLoginExternal, WLClaims.OwnAccount.CanLoginExternal);

                public static WLPolicy CanResetPassword => new(Names.CanResetPassword, WLClaims.OwnAccount.CanResetPassword);

                public static WLPolicy CanChangePassword => new(Names.CanChangePassword, WLClaims.OwnAccount.CanChangePassword);

                public static WLPolicy CanDisable2FA => new(Names.CanDisable2FA, WLClaims.OwnAccount.CanDisable2FA);

                public static WLPolicy CanEnable2FA => new(Names.CanEnable2FA, WLClaims.OwnAccount.CanEnable2FA);
            }
        }

        public static class Administration
        {
            public static class Account
            {
                public static class Names
                {
                    public const string CanResetPassword = "https://weblicense/administration/account/password/reset";

                    public const string CanChangePassword = "https://weblicense/administration/account/password/change";

                    public const string CanDisable2FA = "https://weblicense/administration/account/2fa/disable";

                    public const string CanEnable2FA = "https://weblicense/administration/account/2fa/enable";
                }

                public static class Policies
                {
                    public static WLPolicy CanResetPassword => new(Names.CanResetPassword, WLClaims.Administration.Account.CanResetPassword);

                    public static WLPolicy CanChangePassword => new(Names.CanChangePassword, WLClaims.Administration.Account.CanChangePassword);

                    public static WLPolicy CanDisable2FA => new(Names.CanDisable2FA, WLClaims.Administration.Account.CanDisable2FA);

                    public static WLPolicy CanEnable2FA => new(Names.CanEnable2FA, WLClaims.Administration.Account.CanEnable2FA);
                }
            }
        }
    }

    #region Auxiliary classes

    public class WLPolicy
    {
        public string Name { get; set; }

        public AuthorizationPolicy Policy { get; set; }

        public WLPolicy([NotNull] string name, [NotNull] WLClaim claim)
        {
            Name = !string.IsNullOrWhiteSpace(name) ? name.Trim() : throw new ArgumentNullException(nameof(name));
            Policy = claim?.Policy ?? throw new ArgumentNullException(nameof(claim));
        }
    }

    #endregion
}