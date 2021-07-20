using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WebLicense.Core.Models.Identity
{
    [Index(nameof(RoleId))]
    [Index(nameof(UserId))]
    [Index(nameof(RoleId), nameof(UserId), IsUnique = true)]
    public sealed class UserRole : IdentityUserRole<long>
    {
        // navigation
        public User User { get; set; }
        public Role Role { get; set; }
    }
}