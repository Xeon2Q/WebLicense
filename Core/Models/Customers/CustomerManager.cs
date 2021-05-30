using WebLicense.Core.Models.Identity;

namespace WebLicense.Core.Models.Customers
{
    public sealed class CustomerManager
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public long UserId { get; set; }
        public User User { get; set; }
    }
}