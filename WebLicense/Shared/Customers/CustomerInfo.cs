using System.Collections.Generic;

namespace WebLicense.Shared.Customers
{
    public sealed record CustomerInfo
    {
        public int? Id { get; init; } = null;

        public string Name { get; init; } = null;

        public string Code { get; init; } = null;

        public string ReferenceId { get; init; } = null;

        public CustomerSettingsInfo Settings { get; init; }

        public ICollection<CustomerUserInfo> Administrators { get; set; }

        public ICollection<CustomerUserInfo> Managers { get; set; }

        public ICollection<CustomerUserInfo> Users { get; set; }
    }
}