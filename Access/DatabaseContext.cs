using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Extensions;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WebLicense.Core.Models.Identity;
using UserClaim = WebLicense.Core.Models.Identity.UserClaim;

namespace WebLicense.Access
{
    public sealed class DatabaseContext : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>, IPersistedGrantDbContext
    {
        #region C-tors | Fields

        public DatabaseContext()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DatabaseContext(DbContextOptions options) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DatabaseContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalOptions) : base(options)
        {
            OperationalOptions = operationalOptions.Value;
        }

        #endregion

        #region Properties

        private OperationalStoreOptions OperationalOptions { get; }

        //public DbSet<Log> Logs { get; set; }

        #endregion

        #region IPersistedGrantDbContext implementation

        public DbSet<PersistedGrant> PersistedGrants { get; set; }
        public DbSet<DeviceFlowCodes> DeviceFlowCodes { get; set; }

        public Task<int> SaveChangesAsync() => base.SaveChangesAsync();

        #endregion

        #region Overriden methods

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigurePersistedGrantContext(OperationalOptions);

            builder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
        }

        #endregion
    }
}
