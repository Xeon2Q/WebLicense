using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using WebLicense.Core.Models.Companies;

namespace WebLicense.Core.Models.Identity
{
    public sealed class User : IdentityUser<long>
    {
        public bool EulaAccepted { get; set; }

        public bool GdprAccepted { get; set; }

        // navigation
        public ICollection<Company> Companies { get; set; }
        public ICollection<CompanyUser> CompanyUsers { get; set; }
    }
}