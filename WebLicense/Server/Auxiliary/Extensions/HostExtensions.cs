using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WebLicense.Access;

namespace WebLicense.Server.Auxiliary.Extensions
{
    public static class HostExtensions
    {
        public static IHost ApplyMigrations(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>())
                {
                    var logger = scope.ServiceProvider.GetRequiredService<ILogger<DatabaseContext>>();
                    try
                    {
                        db.Database.Migrate();
                    }
                    catch (Exception e)
                    {
                        logger.LogCritical(e, e.Message);

                        throw;
                    }
                }
            }

            return host;
        }
    }
}