using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using WebLicense.Access;
using WebLicense.Core.Models.Customers;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.Auxiliary.Extensions;
using WebLicense.Logic.UseCases.Auxiliary;

namespace WebLicense.Logic.UseCases.Customers
{
    public sealed class DeleteCustomer : IRequest<CaseResult>, IValidate
    {
        internal int Id { get; }

        public DeleteCustomer(int id)
        {
            Id = id;
        }

        public void Validate()
        {
            if (Id < 1) throw new CaseException("*'Id' must be greater than 0", "'Id' < 1");
        }
    }

    internal sealed class DeleteCustomerHandler : IRequestHandler<DeleteCustomer, CaseResult>
    {
        private readonly DatabaseContext db;

        public DeleteCustomerHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult> Handle(DeleteCustomer request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                db.Detach<Customer>(q => q.Id == request.Id);

                db.Customers.Remove(new Customer {Id = request.Id});
                await db.SaveChangesAsync(cancellationToken);

                return new CaseResult();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (e.Message.Contains("expected to affect 1 row") && e.Message.Contains("actually affected 0 row"))
                {
                    return new CaseResult(new CaseException("*Customer not found or deleted", "Customer not found or deleted"));
                }
                return new CaseResult(e);
            }
            catch (Exception e)
            {
                return new CaseResult(e);
            }
        }
    }
}