using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;
using WebLicense.Server.Auxiliary.Extensions;

namespace WebLicense.Server.Auxiliary.Middlewares
{
    public sealed class LogUserInformationMiddleware
    {
        private readonly RequestDelegate next;

        public LogUserInformationMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var userId = context.User.GetId<long>();
            var userName = context.User.GetName();

            if (userId > 0) LogContext.PushProperty("UserId", userId);
            if (!string.IsNullOrWhiteSpace(userName)) LogContext.PushProperty("UserName", userName);

            return next(context);
        }
    }
}