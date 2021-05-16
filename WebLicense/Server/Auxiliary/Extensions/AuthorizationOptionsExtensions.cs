using System;
using Microsoft.AspNetCore.Authorization;
using WebLicense.Shared.Auxiliary.Policies;

namespace WebLicense.Server.Auxiliary.Extensions
{
    public static class AuthorizationOptionsExtensions
    {
        public static AuthorizationOptions AddPolicy(this AuthorizationOptions options, WLPolicy policy)
        {
            if (policy == null) throw new ArgumentNullException(nameof(policy));

            options.AddPolicy(policy.Name, policy.Policy);
            return options;
        }
    }
}