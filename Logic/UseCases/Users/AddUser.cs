using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebLicense.Access;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Customers;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared.Identity;

namespace WebLicense.Logic.UseCases.Users
{
    public sealed class AddUser : IRequest<CaseResult<UserInfo>>
    {
        internal UserInfo User { get; }
        internal readonly string Password;
        internal readonly string CustomerReferenceId;

        public AddUser(UserInfo user, string password, string customerReferenceId)
        {
            User = user;
            Password = password;
            CustomerReferenceId = !string.IsNullOrWhiteSpace(customerReferenceId) ? customerReferenceId.Trim() : null;

            if (User != null)
            {
                User.Email = !string.IsNullOrWhiteSpace(User.Email) ? User.Email.Trim() : null;
                User.UserName = !string.IsNullOrWhiteSpace(User.UserName) ? User.UserName.Trim() : null;
                User.PhoneNumber = !string.IsNullOrWhiteSpace(User.PhoneNumber) ? User.PhoneNumber.Trim() : null;
            }
        }
    }

    internal sealed class AddUserHandler : IRequestHandler<AddUser, CaseResult<UserInfo>>
    {
        private readonly DatabaseContext db;
        private readonly UserManager<User> userManager;

        public AddUserHandler(DatabaseContext db, UserManager<User> userManager)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<CaseResult<UserInfo>> Handle(AddUser request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                var user = new User
                {
                    Email = request.User.Email,
                    UserName = request.User.UserName,
                    PhoneNumber = request.User.PhoneNumber,
                    EulaAccepted = request.User.EulaAccepted ?? false,
                    GdprAccepted = request.User.GdprAccepted ?? false
                };

                var result1 = await userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
                if (!result1.Succeeded) return new CaseResult<UserInfo>(new UserInfo(user), result1);

                var result2 = await userManager.AddToRoleAsync(user, Roles.CustomerUser).ConfigureAwait(false);
                if (!result2.Succeeded) return new CaseResult<UserInfo>(new UserInfo(user), result2);

                if (!string.IsNullOrWhiteSpace(request.CustomerReferenceId))
                {
                    var customer = await db.Set<Customer>().FirstOrDefaultAsync(q => q.ReferenceId == request.CustomerReferenceId, cancellationToken);
                    if (customer != null)
                    {
                        customer.CustomerUsers.Add(new CustomerUser{UserId = user.Id});
                        await db.SaveChangesAsync(cancellationToken);
                    }
                }

                user = await db.Set<User>().FirstOrDefaultAsync(q => q.Id == user.Id, cancellationToken);

                return new(new UserInfo(user), result1);
            }
            catch (Exception e)
            {
                return new(e);
            }
        }
    }
}