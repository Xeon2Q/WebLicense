using Microsoft.AspNetCore.Identity;

namespace WebLicense.Core.Models.Identity
{
    public sealed class User : IdentityUser<long>
    {
        public bool EulaAccepted { get; set; }

        public bool GdprAccepted { get; set; }
    }
}