using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Server.Auxiliary.Extensions
{
    public static class LoggerExtensions
    {
        public static ILogger With(this ILogger logger, string action, long? userId, string userName)
        {
            var dic = new Dictionary<string, object>(3);
            if (!string.IsNullOrWhiteSpace(action)) dic["Action"] = action?.Trim();
            if (userId > 0) dic["UserId"] = userId.Value;
            if (!string.IsNullOrWhiteSpace(userName)) dic["UserName"] = userName.Trim();

            if (dic.Count == 0) return logger;

            logger.BeginScope(dic);
            return logger;
        }

        public static ILogger With(this ILogger logger, string action, User user)
        {
            return logger.With(action, user?.Id, user?.UserName);
        }

        public static ILogger With(this ILogger logger, string action)
        {
            return logger.With(action, null, null);
        }

        public static ILogger With(this ILogger logger, User user)
        {
            return logger.With(null, user?.Id, user?.UserName);
        }
    }
}