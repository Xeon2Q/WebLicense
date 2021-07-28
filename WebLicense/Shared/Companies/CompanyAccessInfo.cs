using System.Collections.Generic;
using System.Linq;

namespace WebLicense.Shared.Companies
{
    public sealed class CompanyAccessInfo
    {
        public bool HasAccess => IsUserAccess || IsProviderAccess || IsClientAccess;

        public bool IsAdminAccess { get; }

        public bool IsManagerAccess { get; }

        public bool IsUserAccess { get; }

        public bool IsProviderAccess { get; }

        public bool IsClientAccess { get; set; }

        public int[] ViewSettingsAllowedId { get; }

        public int[] EditSettingsAllowedId { get; }

        public int[] ViewClientSettingsAllowedId { get; }

        public int[] EditClientSettingsAllowedId { get; }

        #region C-tor

        public CompanyAccessInfo()
        {
            IsAdminAccess = IsManagerAccess = IsUserAccess = IsProviderAccess = IsClientAccess = false;

            ViewSettingsAllowedId = new int[0];
            EditSettingsAllowedId = new int[0];
        }

        public CompanyAccessInfo(bool isAdmin, bool isManager, bool isUser)
        {
            if (isAdmin)
            {
                IsAdminAccess = IsManagerAccess = IsUserAccess = true;
            }
            else
            {
                IsAdminAccess = false;
                IsManagerAccess = isManager;
                IsUserAccess = isManager || isUser;
            }

            IsProviderAccess = false;
            IsClientAccess = false;
            
            ViewSettingsAllowedId = new int[0];
            EditSettingsAllowedId = new int[0];

            ViewClientSettingsAllowedId = new int[0];
            EditClientSettingsAllowedId = new int[0];
        }

        public CompanyAccessInfo(bool provider, IEnumerable<int> providerViewSettings, IEnumerable<int> providerEditSettings, bool client, IEnumerable<int> clientViewSettings, IEnumerable<int> clientEditSettings)
        {
            IsAdminAccess = false;
            IsManagerAccess = false;
            IsUserAccess = false;

            IsProviderAccess = provider;
            ViewSettingsAllowedId = providerViewSettings?.ToArray() ?? new int[0];
            EditSettingsAllowedId = providerEditSettings?.ToArray() ?? new int[0];

            IsClientAccess = client;
            ViewClientSettingsAllowedId = clientViewSettings?.ToArray() ?? new int[0];
            EditClientSettingsAllowedId = clientEditSettings?.ToArray() ?? new int[0];
        }

        #endregion
    }
}