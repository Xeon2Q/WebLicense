using FluentAssertions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnitTests.Auxiliary;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Customers;
using WebLicense.Shared.Customers;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class CustomerTests : TestBaseAx<CustomerInfo>
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

            var model = await med.Send(new AddCustomer(name, email, userId, adminId));

            model.Should().NotBeNull();

            var data = model.Data;
            data.Should().BeOfType<CustomerInfo>();
            data.Id.Should().BeGreaterThan(0);
            data.Name.Should().BeEquivalentTo(name);
            data.Managers.Should().NotBeNull().And.HaveCount(1).And.Contain(q => q.Id == userId);
            data.Users.Should().NotBeNull().And.HaveCount(1).And.Contain(q => q.Id == userId);
            data.Settings.Should().NotBeNull().And.Match<CustomerSettingsInfo>(q => q.ReceiveNotifications == true && q.NotificationsEmail == email);

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

            var model = await med.Send(new AddCustomer(name, email, userId, adminId));

            model.Should().BeOfType<CaseResult<CustomerInfo>>();
            model.Succeeded.Should().BeFalse();
            model.Data.Should().BeNull();
            model.Errors.Should().NotBeEmpty();

            WriteErrors(model.Errors, output);
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

        [Theory]
        [ClassData(typeof(CustomersValidData))]
        public async Task Update_Success(CustomerInfo info)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            var prev = await med.Send(new GetCustomer(info.Id ?? 0));
            var model = await med.Send(new UpdateCustomer(info));

            model.Should().NotBeNull();
            model.Succeeded.Should().BeTrue();
            model.Data.Should().NotBeNull();
            model.Errors.Should().BeNull();

            CompareModels(model.Data, info, prev.Data);
        }

        [Theory]
        [ClassData(typeof(CustomersFailData))]
        public async Task Update_Fail(CustomerInfo info)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            var model = await med.Send(new UpdateCustomer(info));

            model.Should().NotBeNull();
            model.Succeeded.Should().BeFalse();
            model.Data.Should().BeNull();
            model.Errors.Should().NotBeNullOrEmpty();

            WriteErrors(model.Errors, output);
        }

        [Theory]
        [ClassData(typeof(CustomersExceptionData))]
        public async Task Update_Exception(CustomerInfo info)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            Func<Task> action = async () => await med.Send(new UpdateCustomer(info));

            action.Should().Throw<Exception>();
        }

        #region Methods

        protected override void CompareModels(CustomerInfo @new, CustomerInfo info, CustomerInfo old)
        {
            @new.Should().BeOfType<CustomerInfo>().And.NotBeNull();
            @new.Id.Should().Be(info.Id ?? -9999);

            @new.Code.Should().Be(info.Code ?? old.Code);
            @new.Name.Should().Be(info.Name ?? old.Name);
            @new.ReferenceId.Should().Be(info.ReferenceId ?? old.ReferenceId);

            var ua = (info.Administrators ?? old.Administrators)?.Where(q => q.Id.HasValue).Select(q => q.Id.Value).ToList();
            if (ua == null)
            {
                @new.Administrators.Should().BeNull();
            }
            else
            {
                @new.Administrators.Should().OnlyContain(q => q.Id.HasValue && ua.Contains(q.Id.Value));
            }

            var um = (info.Managers ?? old.Managers)?.Where(q => q.Id.HasValue).Select(q => q.Id.Value).ToList();
            if (um == null)
            {
                @new.Managers.Should().BeNull();
            }
            else
            {
                @new.Managers.Should().OnlyContain(q => q.Id.HasValue && um.Contains(q.Id.Value));
            }
            
            var uu = (info.Users ?? old.Users)?.Where(q => q.Id.HasValue).Select(q => q.Id.Value).ToList();
            if (uu == null)
            {
                @new.Users.Should().BeNull();
            }
            else
            {
                @new.Users.Should().OnlyContain(q => q.Id.HasValue && uu.Contains(q.Id.Value));
            }

            @new.Settings.MaxActiveLicensesCount.Should().Be(info.Settings?.MaxActiveLicensesCount ?? old.Settings.MaxActiveLicensesCount);
            @new.Settings.MaxTotalLicensesCount.Should().Be(info.Settings?.MaxTotalLicensesCount ?? old.Settings.MaxTotalLicensesCount);
            @new.Settings.CreateActiveLicenses.Should().Be(info.Settings?.CreateActiveLicenses ?? old.Settings.CreateActiveLicenses);
            @new.Settings.CanActivateLicenses.Should().Be(info.Settings?.CanActivateLicenses ?? old.Settings.CanActivateLicenses);
            @new.Settings.CanDeactivateLicenses.Should().Be(info.Settings?.CanDeactivateLicenses ?? old.Settings.CanDeactivateLicenses);
            @new.Settings.CanDeleteLicenses.Should().Be(info.Settings?.CanDeleteLicenses ?? old.Settings.CanDeleteLicenses);
            @new.Settings.CanActivateMachines.Should().Be(info.Settings?.CanActivateMachines ?? old.Settings.CanActivateMachines);
            @new.Settings.CanDeactivateMachines.Should().Be(info.Settings?.CanDeactivateMachines ?? old.Settings.CanDeactivateMachines);
            @new.Settings.CanDeleteMachines.Should().Be(info.Settings?.CanDeleteMachines ?? old.Settings.CanDeleteMachines);
            @new.Settings.NotificationsEmail.Should().Be(info.Settings?.NotificationsEmail ?? old.Settings.NotificationsEmail);
            @new.Settings.ReceiveNotifications.Should().Be(info.Settings?.ReceiveNotifications ?? old.Settings.ReceiveNotifications);
        }

        #endregion

        #region Auxiliary classes

        public class CustomersValidData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] {new CustomerInfo {Id = 100}};
                yield return new object[] {new CustomerInfo {Id = 100, Name = "Name X"}};
                yield return new object[] {new CustomerInfo {Id = 100, Code = "Code X"}};
                yield return new object[] {new CustomerInfo {Id = 100, ReferenceId = "Reference X"}};
                yield return new object[] {new CustomerInfo {Id = 100, Administrators = new List<CustomerUserInfo> {new() {Id = 101}, new() {Id = 102}}}};
                yield return new object[] {new CustomerInfo {Id = 100, Managers = new List<CustomerUserInfo> {new() {Id = 101}, new() {Id = 102}}}};
                yield return new object[] {new CustomerInfo {Id = 100, Users = new List<CustomerUserInfo> {new() {Id = 101}, new() {Id = 102}}}};
                yield return new object[] {new CustomerInfo {Id = 100, Settings = new CustomerSettingsInfo {MaxActiveLicensesCount = 9}}};
                yield return new object[] {new CustomerInfo {Id = 100, Settings = new CustomerSettingsInfo {MaxActiveLicensesCount = 9, MaxTotalLicensesCount = 8}}};
                yield return new object[] {new CustomerInfo {Id = 100, Settings = new CustomerSettingsInfo {CanActivateLicenses = true}}};
                yield return new object[] {new CustomerInfo {Id = 100, Settings = new CustomerSettingsInfo {CanDeleteLicenses = true, NotificationsEmail = "foo@baz.bar"}}};
                yield return new object[]
                {
                    new CustomerInfo
                    {
                        Id = 100,
                        Name = "Name Y",
                        Code = "Code Y",
                        ReferenceId = "Ref Y",
                        Administrators = new List<CustomerUserInfo> {new() {Id = 101}},
                        Managers = new List<CustomerUserInfo> {new() {Id = 102}},
                        Users = new List<CustomerUserInfo> {new() {Id = 101}, new() {Id = 102}, new() {Id = 100}},
                        Settings = new CustomerSettingsInfo
                        {
                            MaxActiveLicensesCount = 3,
                            MaxTotalLicensesCount = 5,
                            CreateActiveLicenses = true,
                            CanActivateLicenses = true,
                            CanDeactivateLicenses = true,
                            CanDeleteLicenses = true,
                            CanActivateMachines = true,
                            CanDeactivateMachines = true,
                            CanDeleteMachines = true,
                            NotificationsEmail = "changedEr@ko.gom",
                            ReceiveNotifications = false
                        }
                    }
                };
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class CustomersFailData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] {new CustomerInfo {Id = 603}};
                yield return new object[] {new CustomerInfo {Id = 100, Administrators = new List<CustomerUserInfo> {new() {Id = 701}, new() {Id = 102}}}};
                yield return new object[] {new CustomerInfo {Id = 100, Managers = new List<CustomerUserInfo> {new() {Id = 601}, new() {Id = 102}}}};
                yield return new object[] {new CustomerInfo {Id = 100, Users = new List<CustomerUserInfo> {new() {Id = 501}, new() {Id = 102}}}};
                yield return new object[] {new CustomerInfo {Id = 108, Settings = new CustomerSettingsInfo {MaxActiveLicensesCount = 9}}};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public class CustomersExceptionData : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                yield return new object[] {new CustomerInfo()};
                yield return new object[] {new CustomerInfo {Id = 0, Name = "Name X"}};
                yield return new object[] {new CustomerInfo {Id = -333, Code = "Code X"}};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        #endregion
    }
}
