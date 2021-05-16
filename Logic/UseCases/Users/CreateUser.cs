using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;

namespace WebLicense.Logic.UseCases.Users
{
    public sealed class CreateUser : IRequest<CaseResult<User>>
    {
        internal readonly string UserName;
        internal readonly string Password;
        internal readonly string Phone;
        internal readonly string Email;
        internal readonly bool EULA;
        internal readonly bool GDPR;
        internal readonly string CustomerReferenceId;

        public CreateUser(string userName, string password, string email, string phone, bool eula, bool gdpr, string customerReferenceId)
        {
            UserName = !string.IsNullOrWhiteSpace(userName) ? userName.Trim() : throw new Exception("UserName is empty");
            Password = !string.IsNullOrWhiteSpace(password) ? password : throw new Exception("Password is empty");
            Email = !string.IsNullOrWhiteSpace(email) ? email.Trim() : throw new Exception("Email is empty");
            Phone = !string.IsNullOrWhiteSpace(phone) ? phone.Trim() : null;
            EULA = eula;
            GDPR = gdpr;
            CustomerReferenceId = !string.IsNullOrWhiteSpace(customerReferenceId) ? customerReferenceId.Trim() : null;
        }
    }

    internal sealed class CreateUserHandler : IRequestHandler<CreateUser, CaseResult<User>>
    {
        private readonly UserManager<User> userManager;

        public CreateUserHandler(UserManager<User> userManager)
        {
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<CaseResult<User>> Handle(CreateUser request, CancellationToken cancellationToken)
        {
            try
            {
                var user = new User
                {
                    Email = request.Email,
                    UserName = request.UserName,
                    PhoneNumber = request.Phone,
                    EulaAccepted = request.EULA,
                    GdprAccepted = request.GDPR
                };

                var result1 = await userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
                if (!result1.Succeeded) return new CaseResult<User>(user, result1);

                var result2 = await userManager.AddToRoleAsync(user, Roles.CustomerUser).ConfigureAwait(false);
                if (!result2.Succeeded) return new CaseResult<User>(user, result2);

                return new CaseResult<User>(user, result1);
            }
            catch (Exception e)
            {
                return new CaseResult<User>(e);
            }
        }
    }
}