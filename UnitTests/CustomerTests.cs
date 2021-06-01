using System;
using FluentAssertions;
using System.Threading.Tasks;
using UnitTests.Auxiliary;
using WebLicense.Core.Models.Customers;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Customers;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class CustomerTests : TestBaseAx
    {
        #region C-tor | Fields

        private readonly ITestOutputHelper output;

        public CustomerTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        #endregion

        [Theory]
        [InlineData("Customer 1", "xeon99@gmail.com", 100, 102)]
        [InlineData("Customer 2", "a@g.com", 101, 101)]
        [InlineData("Customer 3", "xeon99@gmail.com", 102, 0)]
        [InlineData("Customer 4", "a@g.com", 100, long.MinValue)]
        public async Task Add_Success(string name, string email, long userId, long adminId)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            var customer = await med.Send(new AddCustomer(name, email, userId, adminId));

            customer.Should().NotBeNull();

            var data = customer.Data;
            data.Should().BeOfType<Customer>();
            data.Id.Should().BeGreaterThan(0);
            data.Name.Should().BeEquivalentTo(name);
            data.Managers.Should().NotBeNull().And.HaveCount(1).And.Contain(q => q.Id == userId);
            data.Users.Should().NotBeNull().And.HaveCount(1).And.Contain(q => q.Id == userId);
            data.Settings.Should().NotBeNull().And.Match<CustomerSettings>(q => q.ReceiveNotifications && q.NotificationsEmail == email);

            if (adminId > 0)
            {
                data.Administrators.Should().NotBeNull().And.HaveCount(1).And.Contain(q => q.Id == adminId);
            }
            else
            {
                data.Administrators.Should().BeNullOrEmpty();
            }
        }

        [Theory]
        [InlineData("Customer 1", "dum@not.exists", 100, 1)]
        [InlineData("Customer 1", "dum@not.exists", 1, 100)]
        [InlineData("Customer 1", "dum@not.exists", 1, 1)]
        public async Task Add_Fail(string name, string email, long userId, long adminId)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            var customer = await med.Send(new AddCustomer(name, email, userId, adminId));

            customer.Should().BeOfType<CaseResult<Customer>>();
            customer.Succeeded.Should().BeFalse();
            customer.Data.Should().BeNull();
            customer.Errors.Should().NotBeEmpty();

            WriteErrors(customer.Errors, output);
        }

        [Theory]
        [InlineData("", "dum@not.exists", 100)]
        [InlineData(null, "dum@not.exists", 100)]
        [InlineData("Customer 1", "", 100)]
        [InlineData("Customer 1", null, 100)]
        [InlineData("Customer 1", "dum@not.exists", -1)]
        [InlineData("Customer 1", "dum@not.exists", 0)]
        public async Task Add_Exception(string name, string email, long userId)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            Func<Task> action = async () => await med.Send(new AddCustomer(name, email, userId));

            action.Should().Throw<Exception>().Where(q => q is ArgumentNullException || q is ArgumentOutOfRangeException);
        }
    }
}
