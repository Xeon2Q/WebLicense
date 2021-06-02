﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading;
using IdentityServer4.EntityFramework.Options;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using WebLicense.Access;
using WebLicense.Core.Models.Customers;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Customers;
using Xunit.Abstractions;

namespace UnitTests.Auxiliary
{
    public abstract class TestBaseAx<T> : IDisposable
    {
        protected const string DOT = "·";

        #region DbContexts

        private DbConnection _connection;

        public DatabaseContext GetMemoryContext(bool useSQLite = true)
        {
            var serviceCollection = new ServiceCollection();
            if (useSQLite) serviceCollection.AddEntityFrameworkSqlite();
            else serviceCollection.AddEntityFrameworkInMemoryDatabase();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            DbContextOptions<DatabaseContext> options;

            if (useSQLite)
            {
                var connection = new SqliteConnection("DataSource=:memory:");
                connection.Open();

                options = new DbContextOptionsBuilder<DatabaseContext>().UseSqlite(connection).UseInternalServiceProvider(serviceProvider).Options;

                _connection = RelationalOptionsExtension.Extract(options).Connection;
            }
            else
            {
                options = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase("InMemoryDatabase-Test").UseInternalServiceProvider(serviceProvider).Options;
            }

            var opOptions = Options.Create(new OperationalStoreOptions());

            var context = new DatabaseContext(options, opOptions);

            try
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            try
            {
                context.Database.Migrate();
            }
            catch
            {
                // ignore
            }

            SeedData(context);

            return context;
        }

        private void SeedData(DatabaseContext db)
        {
            // seed users
            var users = new List<User>
            {
                new() {Id = 100, UserName = "User 100", NormalizedUserName = "User 100".ToUpper(), Email = "user100@mail.net", NormalizedEmail = "user100@mail.net".ToUpper(), EmailConfirmed = true, PhoneNumber = "100-000001", PhoneNumberConfirmed = true, EulaAccepted = true, GdprAccepted = true, SecurityStamp = Guid.NewGuid().ToString("N")},
                new() {Id = 101, UserName = "User 101", NormalizedUserName = "User 101".ToUpper(), Email = "user101@mail.net", NormalizedEmail = "user101@mail.net".ToUpper(), EmailConfirmed = true, PhoneNumber = "101-000002", PhoneNumberConfirmed = true, EulaAccepted = true, GdprAccepted = true, SecurityStamp = Guid.NewGuid().ToString("N")},
                new() {Id = 102, UserName = "User 102", NormalizedUserName = "User 102".ToUpper(), Email = "user102@mail.net", NormalizedEmail = "user102@mail.net".ToUpper(), EmailConfirmed = true, PhoneNumber = "102-000003", PhoneNumberConfirmed = true, EulaAccepted = true, GdprAccepted = true, SecurityStamp = Guid.NewGuid().ToString("N")}
            };
            db.Users.AddRange(users);
            db.SaveChanges();

            // seed customers
            var customers = new List<Customer>
            {
                new(){Id = 100, Name = "Customer 100", Code = "CUS-100", ReferenceId = "CUS-100-REF", Settings = new CustomerSettings(), CustomerUsers = new List<CustomerUser>{new(){UserId = 100}}, CustomerManagers = new List<CustomerManager>{new(){UserId = 100}}},
                new(){Id = 101, Name = "Customer 101", Code = "CUS-101", ReferenceId = "CUS-101-REF", Settings = new CustomerSettings(), CustomerUsers = new List<CustomerUser>{new(){UserId = 101}}, CustomerManagers = new List<CustomerManager>{new(){UserId = 101}}},
                new(){Id = 102, Name = "Customer 102", Code = "CUS-102", ReferenceId = "CUS-102-REF", Settings = new CustomerSettings(), CustomerUsers = new List<CustomerUser>{new(){UserId = 102}}, CustomerManagers = new List<CustomerManager>{new(){UserId = 102}}}
            };
            db.Customers.AddRange(customers);
            db.SaveChanges();
        }

        #endregion

        #region Mediator

        protected IMediator GetMediator(DatabaseContext db = null)
        {
            db ??= GetMemoryContext();

            var mock = new Mock<IMediator>();

            // customers
            mock.Setup(q => q.Send(It.IsAny<GetCustomer>(), It.IsAny<CancellationToken>()))
                .Returns((GetCustomer request, CancellationToken cancellationToken) => new GetCustomerHandler(db).Handle(request, cancellationToken));
            mock.Setup(q => q.Send(It.IsAny<AddCustomer>(), It.IsAny<CancellationToken>()))
                .Returns((AddCustomer request, CancellationToken cancellationToken) => new AddCustomerHandler(db, mock.Object).Handle(request, cancellationToken));
            mock.Setup(q => q.Send(It.IsAny<UpdateCustomer>(), It.IsAny<CancellationToken>()))
                .Returns((UpdateCustomer request, CancellationToken cancellationToken) => new UpdateCustomerHandler(db, mock.Object).Handle(request, cancellationToken));

            return mock.Object;
        }

        #endregion

        #region Support methods

        protected virtual void CompareModels(T @new, T info, T old)
        {
            throw new NotSupportedException();
        }

        protected void WriteErrors(IEnumerable<CaseException> errors, ITestOutputHelper output)
        {
            if (output == null || errors == null) return;

            errors.ToList().ForEach(q => WriteError(q, output));
        }

        protected void WriteError(CaseException error, ITestOutputHelper output)
        {
            if (output == null || error == null) return;

            if (!string.IsNullOrWhiteSpace(error.Message)) output.WriteLine($"{DOT} {error.Message.Trim()}");

            WriteError(error.InnerException, output);

            output.WriteLine("");
        }

        protected void WriteError(Exception exception, ITestOutputHelper output)
        {
            if (output == null || exception == null) return;

            var ex = exception;
            while (ex != null)
            {
                output.WriteLine("--------------------");
                if (!string.IsNullOrWhiteSpace(ex.Message)) output.WriteLine($"{DOT}{DOT} {ex.Message}");
                if (!string.IsNullOrWhiteSpace(ex.StackTrace)) output.WriteLine($"{DOT}{DOT} {ex.StackTrace}");

                ex = ex.InnerException;
            }
        }

        #endregion

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}