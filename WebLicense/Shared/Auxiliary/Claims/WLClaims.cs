using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using WebLicense.Shared.Auxiliary.Policies;

namespace WebLicense.Shared.Auxiliary.Claims
{
    public static class WLClaims
    {
        private const string TRUE_STRING = "true";

        public static class Account
        {
            public static WLClaim LoginExternal => new(WLPolicies.Account.Names.LoginExternal, true, TRUE_STRING);
            public static WLClaim ResetPassword => new (WLPolicies.Account.Names.ResetPassword, true, TRUE_STRING);
            public static WLClaim ChangePassword => new (WLPolicies.Account.Names.ChangePassword, true, TRUE_STRING);
            public static WLClaim Disable2FA => new (WLPolicies.Account.Names.Disable2FA, true, TRUE_STRING);
            public static WLClaim Enable2FA => new (WLPolicies.Account.Names.Enable2FA, true, TRUE_STRING);
        }

        public static class Customer
        {
            public static WLClaim View => new(WLPolicies.Customer.Names.View, true, TRUE_STRING);
            public static WLClaim Edit => new(WLPolicies.Customer.Names.Edit, true, TRUE_STRING);
            public static WLClaim ViewSettings => new(WLPolicies.Customer.Names.ViewSettings, true, TRUE_STRING);
            public static WLClaim EditSettings => new(WLPolicies.Customer.Names.EditSettings, true, TRUE_STRING);
            public static WLClaim ViewAdministrators => new(WLPolicies.Customer.Names.ViewAdministrators, true, TRUE_STRING);
            public static WLClaim EditAdministrators => new(WLPolicies.Customer.Names.EditAdministrators, true, TRUE_STRING);
            public static WLClaim ViewManagers => new(WLPolicies.Customer.Names.ViewManagers, true, TRUE_STRING);
            public static WLClaim EditManagers => new(WLPolicies.Customer.Names.EditManagers, true, TRUE_STRING);
            public static WLClaim ViewUsers => new(WLPolicies.Customer.Names.ViewUsers, true, TRUE_STRING);
            public static WLClaim EditUsers => new(WLPolicies.Customer.Names.EditUsers, true, TRUE_STRING);
        }

        public static class Administration
        {
            public static class Account
            {
                public static WLClaim ResetPassword => new (WLPolicies.Administration.Account.Names.ResetPassword, true, TRUE_STRING);
                public static WLClaim ChangePassword => new (WLPolicies.Administration.Account.Names.ChangePassword, true, TRUE_STRING);
                public static WLClaim Disable2FA => new (WLPolicies.Administration.Account.Names.Disable2FA, true, TRUE_STRING);
                public static WLClaim Enable2FA => new (WLPolicies.Administration.Account.Names.Enable2FA, true, TRUE_STRING);
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