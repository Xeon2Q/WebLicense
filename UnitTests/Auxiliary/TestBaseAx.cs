using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading;
using IdentityServer4.EntityFramework.Options;
using Microsoft.Extensions.Options;
using WebLicense.Access;
using WebLicense.Logic.UseCases.Customers;

namespace UnitTests.Auxiliary
{
    public class TestBaseAx
    {
        #region DbContexts

        public static DatabaseContext GetMemoryContext()
        {
            var serviceProvider = new ServiceCollection().AddEntityFrameworkInMemoryDatabase().BuildServiceProvider();

            var options = new DbContextOptionsBuilder<DatabaseContext>().UseInMemoryDatabase("InMemoryDatabase-Test").UseInternalServiceProvider(serviceProvider).Options;
            var opOptions = Options.Create(new OperationalStoreOptions());

            var context = new DatabaseContext(options, opOptions);

            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            try
            {
                context.Database.Migrate();
            }
            catch
            {
                // ignore
            }

            return context;
        }

        #endregion

        #region Mediator

        protected IMediator GetMediator(DatabaseContext db = null)
        {
            db ??= GetMemoryContext();

            var mock = new Mock<IMediator>();

            // customers
            mock.Setup(q => q.Send(It.IsAny<AddCustomer>(), It.IsAny<CancellationToken>()))
                .Returns((AddCustomer request, CancellationToken cancellationToken) => new AddCustomerHandler(db).Handle(request, cancellationToken));

            return mock.Object;
        }

        #endregion
    }
}