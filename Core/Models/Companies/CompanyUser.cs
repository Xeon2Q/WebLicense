using Microsoft.EntityFrameworkCore;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Core.Models.Companies
{
    [Index(nameof(CompanyId))]
    [Index(nameof(UserId))]
    public sealed class CompanyUser
    {
        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}