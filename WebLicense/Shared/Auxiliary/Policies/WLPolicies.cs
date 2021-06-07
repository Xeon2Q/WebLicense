using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using Microsoft.AspNetCore.Authorization;
using WebLicense.Shared.Auxiliary.Claims;

namespace WebLicense.Shared.Auxiliary.Policies
{
    public static class WLPolicies
    {
        public static class Account
        {
            public static class Names
            {
                public const string LoginExternal = "https://weblicense/account/login/external";
                public const string ResetPassword = "https://weblicense/account/password/reset";
                public const string ChangePassword = "https://weblicense/account/password/change";
                public const string Disable2FA = "https://weblicense/account/2fa/disable";
                public const string Enable2FA = "https://weblicense/account/2fa/enable";
            }

            public static class Policies
            {
                public static WLPolicy LoginExternal => new(Names.LoginExternal, WLClaims.Account.LoginExternal);
                public static WLPolicy ResetPassword => new(Names.ResetPassword, WLClaims.Account.ResetPassword);
                public static WLPolicy ChangePassword => new(Names.ChangePassword, WLClaims.Account.ChangePassword);
                public static WLPolicy Disable2FA => new(Names.Disable2FA, WLClaims.Account.Disable2FA);
                public static WLPolicy Enable2FA => new(Names.Enable2FA, WLClaims.Account.Enable2FA);
            }
        }

        public static class Customer
        {
            public static class Names
            {
                public const string View = "https://weblicense/customer/view";
                public const string Edit = "https://weblicense/customer/edit";
                public const string ViewSettings = "https://weblicense/customer/settings/view";
                public const string EditSettings = "https://weblicense/customer/settings/edit";
                public const string ViewAdministrators = "https://weblicense/customer/administrators/view";
                public const string EditAdministrators = "https://weblicense/customer/administrators/edit";
                public const string ViewManagers = "https://weblicense/customer/managers/view";
                public const string EditManagers = "https://weblicense/customer/managers/edit";
                public const string ViewUsers = "https://weblicense/customer/users/view";
                public const string EditUsers = "https://weblicense/customer/users/edit";
            }

            public static class Policies
            {
                public static WLPolicy View = new(Names.View, WLClaims.Customer.View);
                public static WLPolicy Edit = new(Names.Edit, WLClaims.Customer.Edit);
                public static WLPolicy ViewSettings = new(Names.ViewSettings, WLClaims.Customer.ViewSettings);
                public static WLPolicy EditSettings = new(Names.EditSettings, WLClaims.Customer.EditSettings);
                public static WLPolicy ViewAdministrators = new(Names.ViewAdministrators, WLClaims.Customer.ViewAdministrators);
                public static WLPolicy EditAdministrators = new(Names.EditAdministrators, WLClaims.Customer.EditAdministrators);
                public static WLPolicy ViewManagers = new(Names.ViewManagers, WLClaims.Customer.ViewManagers);
                public static WLPolicy EditManagers = new(Names.EditManagers, WLClaims.Customer.EditManagers);
                public static WLPolicy ViewUsers = new(Names.ViewUsers, WLClaims.Customer.ViewUsers);
                public static WLPolicy EditUsers = new(Names.EditUsers, WLClaims.Customer.EditUsers);
            }
        }

        public static class Administration
        {
            public static class Account
            {
                public static class Names
                {
                    public const string ResetPassword = "https://weblicense/administration/account/password/reset";
                    public const string ChangePassword = "https://weblicense/administration/account/password/change";
                    public const string Disable2FA = "https://weblicense/administration/account/2fa/disable";
                    public const string Enable2FA = "https://weblicense/administration/account/2fa/enable";
                }

                public static class Policies
                {
                    public static WLPolicy ResetPassword => new(Names.ResetPassword, WLClaims.Administration.Account.ResetPassword);
                    public static WLPolicy ChangePassword => new(Names.ChangePassword, WLClaims.Administration.Account.ChangePassword);
                    public static WLPolicy Disable2FA => new(Names.Disable2FA, WLClaims.Administration.Account.Disable2FA);
                    public static WLPolicy Enable2FA => new(Names.Enable2FA, WLClaims.Administration.Account.Enable2FA);
                }
            }

            public static class Customer
            {
                public static class Names
                {
                    public const string Add = "https://weblicense/administration/customer/add";
                    public const string AddOnce = "https://weblicense/administration/customer/add/once!";
                    public const string View = "https://weblicense/administration/customer/view";
                    public const string Edit = "https://weblicense/administration/customer/edit";
                    public const string Delete = "https://weblicense/administration/customer/delete";
                    public const string ViewSettings = "https://weblicense/administration/customer/settings/view";
                    public const string EditSettings = "https://weblicense/administration/customer/settings/edit";
                    public const string ViewAdministrators = "https://weblicense/administration/customer/administrators/view";
                    public const string EditAdministrators = "https://weblicense/administration/customer/administrators/edit";
                    public const string ViewManagers = "https://weblicense/administration/customer/managers/view";
                    public const string EditManagers = "https://weblicense/administration/customer/managers/edit";
                    public const string ViewUsers = "https://weblicense/administration/customer/users/view";
                    public const string EditUsers = "https://weblicense/administration/customer/users/edit";
                }

                public static class Policies
                {
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