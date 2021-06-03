using System;
using System.Linq.Expressions;

namespace WebLicense.Logic.Auxiliary
{
    public sealed class Criteria<T> where T : class
    {
        #region Properties

        public int Skip { get; set; }
        public int Take { get; set; }
        public string Sort { get; set; }
        public bool SortAsc { get; set; }
        public Expression<Func<T, bool>> Filter { get; }

        #endregion

        #region C-tor

        public Criteria(int skip, int take, string sort, bool sortAsc, Expression<Func<T, bool>> filter)
        {
            Skip = skip > 0 ? skip : 0;
            Take = take > 1 ? take : 1;
            Sort = !string.IsNullOrWhiteSpace(sort) ? sort.Trim() : null;
            SortAsc = sortAsc;
            Filter = filter;
        }

        #endregion
    }
}