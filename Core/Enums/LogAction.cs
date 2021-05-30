namespace WebLicense.Core.Enums
{
    public static class LogAction
    {
        public static class Account
        {
            public const string Delete = "Delete account";

            public static class Registration
            {
                public const string Credentials = "Registration";

                public const string External = "Registration (external)";
            }

            public static class Login
            {
                public const string Credentials = "Login attempt";

                public const string External = "Login attempt (external)";

                public const string TwoFactor = "Login attempt (2FA)";

                public const string RecoveryCode = "Login attempt (recovery code)";
            }

            public static class Profile
            {
                public const string Update = "Update profile";

                public const string ChangePassword = "Change password";

                public const string SetPassword = "Set password";

                public const string Disable2FA = "Disable 2FA";

                public const string ForgetBrowser2FA = "2FA forget browser";

                public const string GenerateRecoveryCodes2FA = "Generate 2FA Recovery codes";

                public const string DownloadPersonalData = "Download personal data";

                public const string LinkExternalLogin = "Link External Login";

                public const string RemoveExternalLogin = "Remove External Login";
            }
        }
    }
}