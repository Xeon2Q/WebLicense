using MediatR;
using WebLicense.Logic.Auxiliary;
using WebLicense.Logic.UseCases.Users;

namespace WebLicense.Logic.UseCases.Auxiliary
{
    public static class RequestValidators
    {
        #region Common methods

        private static void ValidateRequest(this IRequest request)
        {
            if (request == null) throw new CaseException("*Request is null", "Request is null");
        }

        private static void ValidateRequest<T>(this IRequest<T> request)
        {
            if (request == null) throw new CaseException("*Request is null", "Request is null");
        }

        #endregion

        #region User requests

        internal static void Validate(this GetUser request)
        {
            request.ValidateRequest();

            if (request.Id < 1) throw new CaseException("*'Id' must be greater than 0", "'Id' < 1");
        }

        internal static void Validate(this AddUser request)
        {
            request.ValidateRequest();

            if (request.User == null) throw new CaseException("*User is null", "User is null");
            if (string.IsNullOrWhiteSpace(request.User.Email)) throw new CaseException("*Email is empty", "Email is empty");
            if (string.IsNullOrWhiteSpace(request.Password)) throw new CaseException("*Password is empty", "Password is empty");
            if (string.IsNullOrWhiteSpace(request.User.UserName)) throw new CaseException("*UserName is empty", "UserName is empty");
            if (request.User.EulaAccepted != true) throw new CaseException("*EULA is not accepted", "EULA is not accepted");
            if (request.User.GdprAccepted != true) throw new CaseException("*GDPR is not accepted", "GDPR is not accepted");
        }

        #endregion
    }
}