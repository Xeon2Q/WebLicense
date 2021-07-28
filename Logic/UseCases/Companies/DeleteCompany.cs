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
        internal long CurrentUserId { get; }

        public DeleteCompany(int id, long currentUserId)
        {
            Id = id;
            CurrentUserId = currentUserId;
        }

        public void Validate()
        {
            if (Id < 1) throw new CaseException(Exceptions.Id_LessOne, "'Id' < 1");
            if (CurrentUserId < 1) throw new CaseException(Exceptions.User_Id_LessOne, "'CurrentUserId' < 1");
        }
    }

    internal sealed class DeleteCompanyHandler : IRequestHandler<DeleteCompany, CaseResult>
    {
        private readonly DatabaseContext db;
        private readonly ISender sender;

        public DeleteCompanyHandler(DatabaseContext db, ISender sender)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public async Task<CaseResult> Handle(DeleteCompany request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var access = await sender.Send(new GetCompanyAccess(request.Id, request.CurrentUserId), cancellationToken);
                if (!access.IsUserAccess) throw new CaseException(Exceptions.Company_NotFoundOrDeleted, $"User({request.CurrentUserId}) does not have permissions to view Company({request.Id})");
                if (!access.IsManagerAccess) throw new CaseException(Exceptions.InsufficientPermissions, $"User({request.CurrentUserId}) does not have permissions to delete Company({request.Id})");

                db.Detach<Company>(q => q.Id == request.Id);
                db.Companies.Remove(new Company {Id = request.Id});
                await db.SaveChangesAsync(cancellationToken);

                return new CaseResult();
            }
            catch (DbUpdateConcurrencyException e)
            {
                if (e.Message.Contains("expected to affect 1 row") && e.Message.Contains("actually affected 0 row"))
                {
                    return new CaseResult(new CaseException(Exceptions.Company_NotFoundOrDeleted, "Company not found or deleted"));
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