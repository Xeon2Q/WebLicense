using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace WebLicense.Logic.Auxiliary.Extensions
{
    internal static class DbContextExtensions
    {
        internal static void Detach<T>(this DbContext context, Func<T, bool> search) where T : class
        {
            if (context == null || search == null) return;

            var entity = context.Set<T>().Local.FirstOrDefault(search);
            if (entity == null) return;

            context.Entry(entity).State = EntityState.Detached;
        }
    }
}