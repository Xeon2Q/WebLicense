using System;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using WebLicense.Core.Models.Identity;

namespace WebLicense.Server.Auxiliary.Extensions
{
    public static class LoggerExtensions
    {
        private static Dictionary<string, object> GetScopeParameters(string logAction, User byUser, User onUser)
        {
            return new()
            {
                {"Action", logAction?.Trim()},

                {"ByUserId", byUser?.Id},
                {"ByUserName", byUser?.UserName?.Trim()},

                {"OnUserId", onUser?.Id},
                {"OnUserName", onUser?.UserName?.Trim()}
            };
        }

        #region Log methods

        public static void LogInformationWith(this ILogger logger, string logAction, User byUser, User onUser, string information, params object[] arguments)
        {
            logger.BeginScope(GetScopeParameters(logAction, byUser, onUser));

            var message = arguments != null && arguments.Length > 0 ? string.Format(information, arguments) : information;
            logger.LogInformation(message);
        }

        public static void LogInformationWith(this ILogger logger, string logAction, User byUser, string information, params object[] arguments) => logger.LogInformationWith(logAction, byUser, null, information, arguments);

        public static void LogWarningWith(this ILogger logger, string logAction, User byUser, User onUser, string warning, params object[] arguments)
        {
            logger.BeginScope(GetScopeParameters(logAction, byUser, onUser));

            var message = arguments != null && arguments.Length > 0 ? string.Format(warning, arguments) : warning;
            logger.LogWarning(message);
        }

        public static void LogWarningWith(this ILogger logger, string logAction, User byUser, string warning, params object[] arguments) => logger.LogWarningWith(logAction, byUser, null, warning, arguments);

        public static void LogErrorWith(this ILogger logger, string logAction, User byUser, User onUser, Exception exception, string error, params object[] arguments)
        {
            logger.BeginScope(GetScopeParameters(logAction, byUser, onUser));

            var message = arguments != null && arguments.Length > 0 ? string.Format(error, arguments) : error;
            logger.LogError(exception, error);
        }

        public static void LogErrorWith(this ILogger logger, string logAction, User byUser, Exception exception, string error, params object[] arguments) => logger.LogErrorWith(logAction, byUser, null, exception, error, arguments);

        public static void LogCriticalWith(this ILogger logger, string logAction, User byUser, User onUser, Exception exception, string critical, params object[] arguments)
        {
            logger.BeginScope(GetScopeParameters(logAction, byUser, onUser));

            var message = arguments != null && arguments.Length > 0 ? string.Format(critical, arguments) : critical;
            logger.LogCritical(exception, message);
        }

        public static void LogCriticalWith(this ILogger logger, string logAction, User byUser, Exception exception, string critical, params object[] arguments) => logger.LogCriticalWith(logAction, byUser, null, exception, critical, arguments);

        #endregion
    }
}