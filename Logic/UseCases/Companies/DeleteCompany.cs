using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Resources;
using WebLicense.Access;
using WebLicense.Core.Models.Companies;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.Auxiliary.Extensions;
using WebLicense.Logic.UseCases.Auxiliary;

namespace WebLicense.Logic.UseCases.Companies
{
    public sealed class DeleteCompany : IRequest<CaseResult>, IValidate
    {
        internal int Id { get; }

        public DeleteCompany(int id)
        {
            Id = id;
        }

        public void Validate()
        {
            if (Id < 1) throw new CaseException(Exceptions.Id_LessOne, "'Id' < 1");
        }
    }

    internal sealed class DeleteCompanyHandler : IRequestHandler<DeleteCompany, CaseResult>
    {
        private readonly DatabaseContext db;

        public DeleteCompanyHandler(DatabaseContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<CaseResult> Handle(DeleteCompany request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                db.Detach<Company>(q => q.Id == request.Id);

                db.Companies.Remove(new Company {Id = request.Id});
                await db.SaveChangesAsync(cancellationToken);

                return new CaseResult();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (e.Message.Contains("expected to affect 1 row") && e.Message.Contains("actually affected 0 row"))
                {
                    return new CaseResult(new CaseException(Exceptions.Company_NotFoundOrDeleted, $"Company({request.Id}) not found or deleted"));
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