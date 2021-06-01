using WebLicense.Core.Models.Customers;

namespace WebLicense.Shared.Customers
{
    public sealed class CustomerSettingsInfo
    {
        public int? MaxActiveLicensesCount { get; init; }

        public int? MaxTotalLicensesCount { get; init; }

        public bool? CreateActiveLicenses { get; init; }

        public bool? CanActivateLicenses { get; init; }

        public bool? CanDeactivateLicenses { get; init; }

        public bool? CanDeleteLicenses { get; init; }

        public bool? CanActivateMachines { get; init; }

        public bool? CanDeactivateMachines { get; init; }

        public bool? CanDeleteMachines { get; init; }

        public string NotificationsEmail { get; init; }

        public bool? ReceiveNotifications { get; init; }

        #region C-tor

        public CustomerSettingsInfo(CustomerSettings settings)
        {
            if (settings == null) return;

            MaxActiveLicensesCount = settings.MaxActiveLicensesCount;
            MaxTotalLicensesCount = settings.MaxTotalLicensesCount;
            CreateActiveLicenses = settings.CreateActiveLicenses;
            CanActivateLicenses = settings.CanActivateLicenses;
            CanDeactivateLicenses = settings.CanDeactivateLicenses;
            CanDeleteLicenses = settings.CanDeleteLicenses;
            CanActivateMachines = settings.CanActivateMachines;
            CanDeactivateMachines = settings.CanDeactivateMachines;
            CanDeleteMachines = settings.CanDeleteMachines;
            NotificationsEmail = settings.NotificationsEmail;
            ReceiveNotifications = settings.ReceiveNotifications;
        }

        #endregion
    }
}