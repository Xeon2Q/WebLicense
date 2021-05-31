using FluentAssertions;
using UnitTests.Auxiliary;
using WebLicense.Logic.UseCases.Customers;
using Xunit;

namespace UnitTests
{
    public class CustomerTests : TestBaseAx
    {
        #region C-tor | Fields

        public CustomerTests()
        {
        }

        #endregion

        [Theory]
        [InlineData("Customer 1", "xeon99@gmail.com", 1, 1)]
        public void Add(string name, string email, long userId, long adminId)
        {
            var db = GetMemoryContext();
            var med = GetMediator(db);

            var customer = med.Send(new AddCustomer(name, email, userId, adminId));

            customer.Should().NotBeNull();
        }
    }
}
