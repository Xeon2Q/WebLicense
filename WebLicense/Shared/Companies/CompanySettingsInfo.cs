using System.ComponentModel.DataAnnotations;
using WebLicense.Core.Models.Companies;
using WebLicense.Shared.Resources;

namespace WebLicense.Shared.Companies
{
    public sealed class CompanySettingsInfo
    {
        [Required]
        public int? ServiceProviderCompanyId { get; set; }

        [Required]
        public int? ServiceConsumerCompanyId { get; set; }

        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Model), Name = "Customer_Settings_MaxActiveLicensesCount")]
        public int? MaxActiveLicensesCount { get; set; }

        [Range(0, int.MaxValue)]
        [Display(ResourceType = typeof(Model), Name = "Customer_Settings_MaxTotalLicensesCount")]
        public int? MaxTotalLicensesCount { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Customer_Settings_CreateActiveLicenses")]
        public bool? CreateActiveLicenses { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Customer_Settings_CanActivateLicenses")]
        public bool? CanActivateLicenses { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Customer_Settings_CanDeactivateLicenses")]
        public bool? CanDeactivateLicenses { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Customer_Settings_CanDeleteLicenses")]
        public bool? CanDeleteLicenses { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Customer_Settings_CanActivateMachines")]
        public bool? CanActivateMachines { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Customer_Settings_CanDeactivateMachines")]
        public bool? CanDeactivateMachines { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Customer_Settings_CanDeleteMachines")]
        public bool? CanDeleteMachines { get; set; }

        [Required]
        [Display(ResourceType = typeof(Model), Name = "Customer_Settings_NotificationsEmail")]
        public string NotificationsEmail { get; set; }

        [Display(ResourceType = typeof(Model), Name = "Customer_Settings_ReceiveNotifications")]
        public bool? ReceiveNotifications { get; set; }

        #region C-tor

        public CompanySettingsInfo()
        {
        }

        public CompanySettingsInfo(CompanySettings settings)
        {
            if (settings == null) return;

            ServiceProviderCompanyId = settings.ServiceProviderCompanyId;
            ServiceConsumerCompanyId = settings.ServiceConsumerCompanyId;

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