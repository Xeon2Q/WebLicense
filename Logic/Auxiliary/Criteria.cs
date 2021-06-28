using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Net;
using System.Text.Json;

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

        public Criteria(int skip, int take, CriteriaSort sort, params CriteriaFilter[] filters) : this(skip, take, sort?.Property, sort?.SortOrder == null || sort.SortOrder == CriteriaSortOrder.Asc, null)
        {
        }

        #endregion
    }

    #region Auxiliary classes

    public sealed class CriteriaFilter
    {
        #region Properties

        public string Property { get; set; }

        public string FilterValue { get; set; }

        public CriteriaFilterOperator FilterOperator { get; set; }

        public string SecondFilterValue { get; set; }

        public CriteriaFilterOperator SecondFilterOperator { get; set; }

        public CriteriaLogicalFilterOperator LogicalFilterOperator { get; set; }

        #endregion

        #region Methods

        public static IList<CriteriaFilter> TryParse(string @string)
        {
            if (string.IsNullOrWhiteSpace(@string)) return null;

            try
            {
                return JsonSerializer.Deserialize<IList<CriteriaFilter>>(WebUtility.UrlDecode(@string));
            }
            catch (Exception)
            {
                /* ignore */
            }

            try
            {
                return new List<CriteriaFilter> {JsonSerializer.Deserialize<CriteriaFilter>(WebUtility.UrlDecode(@string))};
            }
            catch (Exception)
            {
                /* ignore */
            }

            return null;
        }

        #endregion
    }

    public sealed class CriteriaSort
    {
        #region Properties

        public string Property { get; set; }

        public CriteriaSortOrder SortOrder { get; set; }

        #endregion

        #region Methods

        public static IList<CriteriaSort> TryParse(string @string)
        {
            if (string.IsNullOrWhiteSpace(@string)) return null;

            try
            {
                return JsonSerializer.Deserialize<IList<CriteriaSort>>(WebUtility.UrlDecode(@string));
            }
            catch (Exception)
            {
                /* ignore */
            }

            try
            {
                return new List<CriteriaSort> {JsonSerializer.Deserialize<CriteriaSort>(WebUtility.UrlDecode(@string))};
            }
            catch (Exception)
            {
                /* ignore */
            }

            return null;
        }

        #endregion
    }

    #region Auxiliary enums

    public enum CriteriaFilterOperator
    {
        Equals = 0,
        NotEquals = 1,
        Contains = 6,
        StartsWith = 7,
        EndsWith = 8
    }

    public enum CriteriaLogicalFilterOperator
    {
        And = 0,
        Or = 1
    }

    public enum CriteriaSortOrder
    {
        Asc = 0,
        Desc = 1
    }

    #endregion

    #endregion
}