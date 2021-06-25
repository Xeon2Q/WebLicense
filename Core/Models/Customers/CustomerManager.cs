using Microsoft.EntityFrameworkCore;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Core.Models.Customers
{
    [Index(nameof(CustomerId))]
    [Index(nameof(UserId))]
    public sealed class CustomerManager
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}