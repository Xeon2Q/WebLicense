using System;

namespace WebLicense.Core.Auxiliary
{
    #region SMTP settings

    public sealed class SmtpSettings
    {
        public FromSettings From { get; set; } = new();

        public sealed class FromSettings
        {
            public string Name { get; set; }

            public string Email { get; set; }
        }

        public ServerSettings Server { get; set; } = new();

        public sealed class ServerSettings
        {
            public string Name { get; set; }

            public int Port { get; set; }

            public bool UseSSL { get; set; }

            public string Login { get; set; }

            public string Password { get; set; }
        }
    }

    #endregion

    #region Identity settings

    public sealed class IdentitySettings
    {
        public UserSettings User { get; set; } = new();

        public sealed class UserSettings
        {
            public string AllowedUserNameCharacters { get; set; }

            public bool RequireUniqueEmail { get; set; }
        }

        public PasswordSettings Password { get; set; } = new();

        public sealed class PasswordSettings
        {
            public bool RequireDigit { get; set; }

            public bool RequireLowercase { get; set; }

            public bool RequireUppercase { get; set; }

            public bool RequireNonAlphanumeric { get; set; }

            public int RequiredUniqueChars { get; set; }

            public int RequiredLength { get; set; }

        }

        public SignInSettings SignIn { get; set; } = new();

        public sealed class SignInSettings
        {
            public bool RequireConfirmedEmail { get; set; }

            public bool RequireConfirmedPhoneNumber { get; set; }

            public bool RequireConfirmedAccount { get; set; }
        }

        public LockoutSettings Lockout { get; set; } = new();

        public sealed class LockoutSettings
        {
            public bool AllowedForNewUsers { get; set; } = true;

            public int MaxFailedAccessAttempts { get; set; } = 5;

            public TimeSpan DefaultLockoutTimeSpan { get; set; } = TimeSpan.FromMinutes(1);
        }
    }

    #endregion

    #region External authentication

    public sealed class AuthenticationSettings
    {
        public AuthenticationProvider Microsoft { get; set; }

        public sealed class AuthenticationProvider
        {
            public string Id { get; set; }

            public string Secret { get; set; }
        }
    }

    #endregion
}