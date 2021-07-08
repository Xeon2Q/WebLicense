using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Resources;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebLicense.Access;
using WebLicense.Core.Enums;
using WebLicense.Core.Models.Companies;
using WebLicense.Core.Models.Identity;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.Auxiliary.Extensions;
using WebLicense.Logic.UseCases.Auxiliary;
using WebLicense.Shared.Identity;

namespace WebLicense.Logic.UseCases.Users
{
    public sealed class AddUser : IRequest<CaseResult<User>>, IValidate
    {
        internal UserInfo User { get; }
        internal readonly string Password;
        internal readonly string CompanyReferenceId;

        public AddUser(UserInfo user, string password, string companyReferenceId)
        {
            User = user;
            Password = password;
            CompanyReferenceId = !string.IsNullOrWhiteSpace(companyReferenceId) ? companyReferenceId.Trim() : null;

            if (User != null)
            {
                User.Email = !string.IsNullOrWhiteSpace(User.Email) ? User.Email.Trim() : null;
                User.UserName = !string.IsNullOrWhiteSpace(User.UserName) ? User.UserName.Trim() : null;
                User.PhoneNumber = !string.IsNullOrWhiteSpace(User.PhoneNumber) ? User.PhoneNumber.Trim() : null;
            }
        }

        public void Validate()
        {
            if (User == null) throw new CaseException(Exceptions.User_Null, "User is null");
            if (string.IsNullOrWhiteSpace(User.Email)) throw new CaseException(Exceptions.User_Email_Empty, "User 'Email' is null or empty");
            if (string.IsNullOrWhiteSpace(Password)) throw new CaseException(Exceptions.User_Password_Empty, "User 'Password' is null or empty");
            if (string.IsNullOrWhiteSpace(User.UserName)) throw new CaseException(Exceptions.User_Name_Empty, "User 'Name' is null or empty");
            if (User.EulaAccepted != true) throw new CaseException(Exceptions.User_Eula_NotAccepted, "User 'EULA' is not accepted");
            if (User.GdprAccepted != true) throw new CaseException(Exceptions.User_Gdpr_NotAccepted, "User 'GDPR' is not accepted");
        }
    }

    internal sealed class AddUserHandler : IRequestHandler<AddUser, CaseResult<User>>
    {
        private readonly DatabaseContext db;
        private readonly UserManager<User> userManager;

        public AddUserHandler(DatabaseContext db, UserManager<User> userManager)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task<CaseResult<User>> Handle(AddUser request, CancellationToken cancellationToken)
        {
            try
            {
                request.Validate();

                // create transaction
                await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);

                // create user
                var user = GetEmptyUser(request.User);
                var result1 = await userManager.CreateAsync(user, request.Password).ConfigureAwait(false);
                if (!result1.Succeeded) return new CaseResult<User>(user, result1);

                // create role
                var result2 = await userManager.AddToRoleAsync(user, Roles.CustomerUser).ConfigureAwait(false);
                if (!result2.Succeeded) return new CaseResult<User>(user, result2);

                // create or attach company
                if (string.IsNullOrWhiteSpace(request.CompanyReferenceId))
                {
                    var company = GetEmptyCompany(user);
                    user.CompanyUsers.Add(new() {Company = company, User = user, IsManager = true});
                }
                else
                {
                    var company = await db.Set<Company>().FirstOrDefaultAsync(q => q.ReferenceId == request.CompanyReferenceId, cancellationToken);
                    user.CompanyUsers.Add(new() {Company = company, User = user, IsManager = false});
                }

                // save & commit changes
                await db.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                user = await db.Set<User>().FirstOrDefaultAsync(q => q.Id == user.Id, cancellationToken);

                return new(user, result1);
            }
            catch (Exception e)
            {
                return new(e);
            }
        }

        #region Methods

        private static User GetEmptyUser(UserInfo user)
        {
            if (user == null) return null;

            return new User
            {
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                EulaAccepted = user.EulaAccepted ?? false,
                GdprAccepted = user.GdprAccepted ?? false,
                CompanyUsers = new List<CompanyUser>()
            };
        }

        private static Company GetEmptyCompany(User user)
        {
            return new()
            {
                Name = $"{user.UserName}-{Guid.NewGuid():N}",
                Code = string.Empty.GetRandom(50),
                ReferenceId = Guid.NewGuid().ToString("N"),
                Logo = null,
                Settings = null
            };
        }

        #endregion
    }
}