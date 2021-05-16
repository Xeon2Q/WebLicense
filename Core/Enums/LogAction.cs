namespace WebLicense.Core.Enums
{
    public static class LogAction
    {
        public const string LoginAttempt = "Login attempt";

        public const string UpdateOwnProfile = "Update profile (self)";

        public const string ChangeOwnPassword = "Change password (self)";

        public const string SetOwnPassword = "Set password (self)";

        public const string ForgetBrowser2Fa = "2FA forget browser (self)";

        public const string DownloadPersonalData = "Download personal data (self)";

        public const string DeleteAccount = "Delete account (self)";

        public const string Disable2FA = "Disable 2FA (self)";

        public const string Generate2FARecoveryCodes = "Generate 2FA Recovery codes (self)";

        public const string RemoveExternalLogin = "Remove External Login (self)";

        public const string LinkExternalLogin = "Link External Login (self)";
    }
}