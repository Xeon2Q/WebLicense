using System;
using FluentAssertions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UnitTests.Auxiliary;
using WebLicense.Core.Models.Customers;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Customers;
using WebLicense.Shared.Customers;
using Xunit;
using Xunit.Abstractions;

namespace UnitTests
{
    public class CustomerTests : TestBaseAx<CustomerInfo>
    {
        #region C-tor | Fields | Properties

        public CustomerTests(ITestOutputHelper output) : base(output)
        {
        }

        private static readonly IDictionary<string, Expression<Func<Customer, bool>>> FiltersValid = new Dictionary<string, Expression<Func<Customer, bool>>>
        {
            {"", null},
            {"id-101", q => q.Id == 101},
            {"name-102", q => q.Name == "Customer 102"},
            {"miss", q => q.Code == "missing code"}
        };

        #endregion

        [Theory]
        [InlineData(100)]
        [InlineData(102)]
        [Trait("Customer", "Get")]
        public async Task Get_Success(int id)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            var model = await med.Send(new GetCustomer(id));

            model.Should().NotBeNull();
            model.Succeeded.Should().BeTrue();
            model.Data.Should().NotBeNull().And.BeOfType<CustomerInfo>();
            model.Errors.Should().BeNull();
        }

        [Theory]
        [InlineData(-9999)]
        [InlineData(0)]
        [InlineData(9999)]
        [Trait("Customer", "Get")]
        public async Task Get_Fail(int id)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            var model = await med.Send(new GetCustomer(id));

            model.Should().NotBeNull();
            model.Succeeded.Should().BeFalse();
            model.Data.Should().BeNull();
            model.Errors.Should().NotBeNullOrEmpty();

            WriteErrors(model.Errors);
        }

        [Theory]
        [InlineData(0, 25, "id", true, "")]
        [InlineData(0, 1, "", true, "")]
        [InlineData(1, 1, "", true, "")]
        [InlineData(1, 25, "name", false, "")]
        [InlineData(0, 25, "name", true, "id-101")]
        [InlineData(0, 3, "name", true, "name-102")]
        [InlineData(0, 3, "name", true, "miss")]
        [Trait("Customer", "Get")]
        public async Task GetAll_Success(int skip, int take, string sort, bool asc, string filterName)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            var filter = FiltersValid[filterName];
            var model = await med.Send(new GetCustomers(skip, take, sort, asc, filter));

            model.Should().NotBeNull();
            model.Succeeded.Should().BeTrue();
            if (filterName != "miss")
                model.Data.Should().NotBeNullOrEmpty().And.BeAssignableTo<IList<CustomerInfo>>();
            else
                model.Data.Should().BeNullOrEmpty().And.BeAssignableTo<IList<CustomerInfo>>();
            model.Errors.Should().BeNull();
            model.Data.Should().HaveCountLessOrEqualTo(new[] {take, 3 - skip}.Min());

            if (!string.IsNullOrWhiteSpace(sort))
            {
                if ("id".Equals(sort, StringComparison.OrdinalIgnoreCase))
                {
                    if (asc)
                        model.Data.Should().BeInAscendingOrder(q => q.Id);
                    else
                        model.Data.Should().BeInDescendingOrder(q => q.Id);
                }
                else if ("name".Equals(sort, StringComparison.OrdinalIgnoreCase))
                {
                    if (asc)
                        model.Data.Should().BeInAscendingOrder(q => q.Name);
                    else
                        model.Data.Should().BeInDescendingOrder(q => q.Name);
                }
                else
                {
                    model.Data.Should().BeInAscendingOrder(q => q.Name);
                }
            }
            else
            {
                model.Data.Should().BeInAscendingOrder(q => q.Name);
            }
        }

        [Theory]
        [InlineData("Customer 1", "xeon99@gmail.com", 100, 102)]
        [InlineData("Customer 2", "a@g.com", 101, 101)]
        [InlineData("Customer 3", "xeon99@gmail.com", 102, 0)]
        [InlineData("Customer 4", "a@g.com", 100, long.MinValue)]
        [Trait("Customer", "Add/Update")]
        public async Task Add_Success(string name, string email, long userId, long adminId)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            var model = await med.Send(new AddCustomer(new CustomerInfo
            {
                Name = name,
                Settings = new CustomerSettingsInfo {NotificationsEmail = email},
                Users = new List<CustomerUserInfo> {new() {Id = userId}},
                Managers = new List<CustomerUserInfo> {new() {Id = userId}},
                Administrators = new List<CustomerUserInfo> {new() {Id = adminId}}
            }));

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
        [InlineData("", "dum@not.exists", 100, 0)]
        [InlineData(null, "dum@not.exists", 100, 0)]
        [InlineData("Customer 1", "", 100, 0)]
        [InlineData("Customer 1", null, 100, 0)]
        [InlineData("Customer 1", "dum@not.exists", -1, 0)]
        [InlineData("Customer 1", "dum@not.exists", 0, 0)]
        [Trait("Customer", "Add/Update")]
        public async Task Add_Fail(string name, string email, long userId, long adminId)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            var model = await med.Send(new AddCustomer(new CustomerInfo
            {
                Name = name,
                Settings = new CustomerSettingsInfo {NotificationsEmail = email},
                Users = new List<CustomerUserInfo> {new() {Id = userId}},
                Managers = new List<CustomerUserInfo> {new() {Id = userId}},
                Administrators = new List<CustomerUserInfo> {new() {Id = adminId}}
            }));

            model.Should().BeOfType<CaseResult<CustomerInfo>>();
            model.Succeeded.Should().BeFalse();
            model.Data.Should().BeNull();
            model.Errors.Should().NotBeEmpty();

            WriteErrors(model.Errors);
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

            WriteErrors(model.Errors);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(101)]
        [InlineData(102)]
        public async Task Delete_Success(int id)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            var model = await med.Send(new DeleteCustomer(id));

            model.Should().NotBeNull();
            model.Succeeded.Should().BeTrue();
            model.Errors.Should().BeNullOrEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        [InlineData(1100)]
        public async Task Delete_Fail(int id)
        {
            await using var db = GetMemoryContext();
            var med = GetMediator(db);

            var model = await med.Send(new DeleteCustomer(id));

            model.Should().NotBeNull();
            model.Succeeded.Should().BeFalse();
            model.Errors.Should().NotBeNullOrEmpty();

            WriteErrors(model.Errors);
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
                yield return new object[] {new CustomerInfo()};
                yield return new object[] {new CustomerInfo {Id = 0, Name = "Name X"}};
                yield return new object[] {new CustomerInfo {Id = -333, Code = "Code X"}};
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        #endregion
    }
}
