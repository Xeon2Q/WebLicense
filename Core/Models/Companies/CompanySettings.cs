using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebLicense.Core.Models.Companies
{
    [Table("CompanySettings")]
    [Index(nameof(ReceiveNotifications))]
    public sealed class CompanySettings
    {
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public int ProviderCompanyId { get; set; }
        public Company ProviderCompany { get; set; }

        [Range(0, int.MaxValue)]
        public int MaxActiveLicensesCount { get; set; } = 1;

        [Range(0, int.MaxValue)]
        public int MaxTotalLicensesCount { get; set; } = 1;

        [DefaultValue(false)]
        public bool CreateActiveLicenses { get; set; } = false;

        [DefaultValue(false)]
        public bool CanActivateLicenses { get; set; } = false;

        [DefaultValue(false)]
        public bool CanDeactivateLicenses { get; set; } = false;

        [DefaultValue(false)]
        public bool CanDeleteLicenses { get; set; } = false;

        [DefaultValue(false)]
        public bool CanActivateMachines { get; set; } = false;

        [DefaultValue(false)]
        public bool CanDeactivateMachines { get; set; } = false;

        [DefaultValue(false)]
        public bool CanDeleteMachines { get; set; } = false;

        [MaxLength(200), Comment("Email address used for notifications.")]
        public string NotificationsEmail { get; set; }

        [Comment("Must be TRUE if Customer wants to receive notifications about his licenses.")]
        public bool ReceiveNotifications { get; set; } = true;
    }
}