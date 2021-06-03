using WebLicense.Core.Models.Customers;

namespace WebLicense.Shared.Identity
{
    public class UserCustomerInfo
    {
        public int? Id { get; init; }

        public string Name { get; init; }

        public string Code { get; init; }

        #region C-tor

        public UserCustomerInfo()
        {
        }

        public UserCustomerInfo(Customer customer)
        {
            if (customer == null) return;

            Id = customer.Id;
            Name = customer.Name;
            Code = customer.Code;
        }

        #endregion
    }
}