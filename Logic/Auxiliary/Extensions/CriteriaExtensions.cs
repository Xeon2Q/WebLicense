using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebLicense.Access;

namespace WebLicense.Logic.Auxiliary.Extensions
{
    internal static class CriteriaExtensions
    {
        internal static async Task<int> GetTotal<T>(this Criteria<T> criteria, DatabaseContext db, CancellationToken cancellationToken) where T : class
        {
            if (db == null) return 0;

            var query = db.Set<T>().AsNoTrackingWithIdentityResolution();

            return await query.CountAsync(cancellationToken);
        }

        internal static async Task<int> GetTotalFiltered<T>(this Criteria<T> criteria, DatabaseContext db, int total, CancellationToken cancellationToken) where T : class
        {
            if (criteria?.Filter == null || db == null) return total;

            var query = db.Set<T>().AsNoTrackingWithIdentityResolution();
            criteria.ApplyFiltering(ref query);

            return await query.CountAsync(cancellationToken);
        }

        internal static async Task<IEnumerable<T>> GetData<T>(this Criteria<T> criteria, DatabaseContext db, CancellationToken cancellationToken) where T : class
        {
            if (db == null) return null;

            var query = db.Set<T>().AsNoTrackingWithIdentityResolution();
            criteria?.ApplyAll(ref query);

            return await query.ToListAsync(cancellationToken);
        }
    }
}