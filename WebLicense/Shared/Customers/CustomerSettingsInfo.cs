using WebLicense.Core.Models.Customers;

namespace WebLicense.Shared.Customers
{
    public sealed class CustomerSettingsInfo
    {
        public int? MaxActiveLicensesCount { get; set; }

        public int? MaxTotalLicensesCount { get; set; }

        public bool? CreateActiveLicenses { get; set; }

        public bool? CanActivateLicenses { get; set; }

        public bool? CanDeactivateLicenses { get; set; }

        public bool? CanDeleteLicenses { get; set; }

        public bool? CanActivateMachines { get; set; }

        public bool? CanDeactivateMachines { get; set; }

        public bool? CanDeleteMachines { get; set; }

        public string NotificationsEmail { get; set; }

        public bool? ReceiveNotifications { get; set; }

        #region C-tor

        public CustomerSettingsInfo()
        {
        }

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