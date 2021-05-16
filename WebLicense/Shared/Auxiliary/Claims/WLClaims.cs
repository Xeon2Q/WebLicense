using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebLicense.Shared.Auxiliary.Policies;

namespace WebLicense.Shared.Auxiliary.Claims
{
    public static class WLClaims
    {
        private const string TRUE_STRING = "true";

        public static class OwnAccount
        {
            public static WLClaim CanLoginExternal => new(WLPolicies.OwnAccount.Names.CanLoginExternal, true, TRUE_STRING);

            public static WLClaim CanResetPassword => new (WLPolicies.OwnAccount.Names.CanResetPassword, true, TRUE_STRING);

            public static WLClaim CanChangePassword => new (WLPolicies.OwnAccount.Names.CanChangePassword, true, TRUE_STRING);

            public static WLClaim CanDisable2FA => new (WLPolicies.OwnAccount.Names.CanDisable2FA, true, TRUE_STRING);

            public static WLClaim CanEnable2FA => new (WLPolicies.OwnAccount.Names.CanEnable2FA, true, TRUE_STRING);
        }

        public static class Administration
        {
            public static class Account
            {
                public static WLClaim CanResetPassword => new (WLPolicies.Administration.Account.Names.CanResetPassword, true, TRUE_STRING);

                public static WLClaim CanChangePassword => new (WLPolicies.Administration.Account.Names.CanChangePassword, true, TRUE_STRING);

                public static WLClaim CanDisable2FA => new (WLPolicies.Administration.Account.Names.CanDisable2FA, true, TRUE_STRING);

                public static WLClaim CanEnable2FA => new (WLPolicies.Administration.Account.Names.CanEnable2FA, true, TRUE_STRING);
            }
        }
    }

    #region Auxiliary classes

    public class WLClaim
    {
        public string ClaimType { get; }

        public Claim Claim { get; }

        public AuthorizationPolicy Policy { get; }

        private WLClaim(string claimType, params string[] acceptedValues)
        {
            if (string.IsNullOrWhiteSpace(claimType)) throw new ArgumentNullException(nameof(claimType));

            ClaimType = claimType.Trim();
            Policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireClaim(ClaimType, acceptedValues).Build();
        }

        internal WLClaim(string claimType, string claimValue, params string[] acceptedValues) : this(claimType, acceptedValues)
        {
            Claim = new Claim(ClaimType, claimValue, ClaimValueTypes.String);
        }

        internal WLClaim(string claimType, bool claimValue, params string[] acceptedValues) : this(claimType, acceptedValues)
        {
            Claim = new Claim(ClaimType, claimValue ? "true" : "false", ClaimValueTypes.Boolean);
        }
    }

    #endregion
}