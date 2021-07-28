using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebLicense.Access;

namespace WebLicense.Logic.Auxiliary.Extensions
{
    internal static class CriteriaExtensions
    {
        internal static async Task<int> GetTotal<T>(this Criteria<T> criteria, DatabaseContext db, CancellationToken cancellationToken) where T : class =>
            await criteria.GetTotal(db?.Set<T>().AsNoTrackingWithIdentityResolution(), cancellationToken);

        internal static async Task<int> GetTotal<T>(this Criteria<T> criteria, IQueryable<T> query, CancellationToken cancellationToken) where T : class
        {
            return query != null
                ? await query.CountAsync(cancellationToken)
                : 0;
        }

        internal static async Task<int> GetTotalFiltered<T>(this Criteria<T> criteria, DatabaseContext db, int total, CancellationToken cancellationToken) where T : class =>
            await criteria.GetTotalFiltered(db?.Set<T>().AsNoTrackingWithIdentityResolution(), total, cancellationToken);

        internal static async Task<int> GetTotalFiltered<T>(this Criteria<T> criteria, IQueryable<T> query, int total, CancellationToken cancellationToken) where T : class
        {
            if (criteria?.Filter == null || query == null) return total;

            criteria.ApplyFiltering(ref query);

            return await query.CountAsync(cancellationToken);
        }

        internal static async Task<IEnumerable<T>> GetData<T>(this Criteria<T> criteria, DatabaseContext db, CancellationToken cancellationToken) where T : class =>
            await criteria.GetData(db?.Set<T>().AsNoTrackingWithIdentityResolution(), cancellationToken);

        internal static async Task<IEnumerable<T>> GetData<T>(this Criteria<T> criteria, IQueryable<T> query, CancellationToken cancellationToken) where T : class
        {
            if (query == null) return null;

            criteria?.ApplyAll(ref query);

            return await query.ToListAsync(cancellationToken);
        }
    }
}