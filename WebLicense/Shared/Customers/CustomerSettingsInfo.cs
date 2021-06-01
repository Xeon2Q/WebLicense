namespace WebLicense.Shared.Customers
{
    public sealed class CustomerSettingsInfo
    {
        public int? MaxActiveLicensesCount { get; init; } = null;

        public int? MaxTotalLicensesCount { get; init; } = null;

        public bool? CreateActiveLicenses { get; init; } = null;

        public bool? CanActivateLicenses { get; init; } = null;

        public bool? CanDeactivateLicenses { get; init; } = null;

        public bool? CanDeleteLicenses { get; init; } = null;

        public bool? CanActivateMachine { get; init; } = null;

        public bool? CanDeactivateMachine { get; init; } = null;

        public bool? CanDeleteMachine { get; init; } = null;

        public string NotificationsEmail { get; init; }

        public bool? ReceiveNotifications { get; init; } = null;
    }
}