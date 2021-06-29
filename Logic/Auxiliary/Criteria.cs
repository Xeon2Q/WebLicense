using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace WebLicense.Logic.Auxiliary
{
    public sealed class Criteria<T> where T : class
    {
        #region Properties

        private PropertyInfo[] Properties { get; } = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public);

        public int Skip { get; set; }
        public int Take { get; set; }
        public CriteriaSort[] Sort { get; set; }
        public Expression<Func<T, bool>> Filter { get; }

        #endregion

        #region C-tor

        public Criteria(int skip, int take)
        {
            Skip = skip > 0 ? skip : 0;
            Take = take > 1 ? take : 1;
        }

        public Criteria(int skip, int take, string sort, bool sortAsc, Expression<Func<T, bool>> filter) : this(skip, take)
        {
            Filter = filter;
            if (!string.IsNullOrWhiteSpace(sort)) Sort = new CriteriaSort[] {new(sort, sortAsc ? CriteriaSortOrder.Asc : CriteriaSortOrder.Desc)};
        }

        public Criteria(int skip, int take, IEnumerable<CriteriaSort> sort, IEnumerable<CriteriaFilter> filters) : this(skip, take)
        {
            Sort = sort?.Where(q => !string.IsNullOrWhiteSpace(q.Property)).ToArray();
            Filter = GetExpressionFromFilters(filters?.ToArray());
        }

        #endregion

        #region Methods

        public void ApplyAll(ref IQueryable<T> query)
        {
            if (query == null) return;

            ApplyFiltering(ref query);
            ApplySorting(ref query);
            ApplyPaging(ref query);
        }

        public void ApplyFiltering(ref IQueryable<T> query)
        {
            if (query == null || Filter == null) return;

            query = query.Where(Filter);
        }

        public void ApplySorting(ref IQueryable<T> query)
        {
            if (query == null || Sort == null) return;

            var sorts = Sort.Where(q => Properties.Any(w => w.Name == q.Property)).ToArray();
            if (sorts.Length == 0) return;

            // apply single sort
            ApplyCriteriaSort(ref query, sorts[0]);

            // apply multiple sorts
            if (query is IOrderedQueryable<T> orderedQuery)
            {
                for (var i = 1; i < sorts.Length; i++)
                {
                    ApplyCriteriaSort(ref orderedQuery, sorts[i]);
                }

                query = orderedQuery;
            }
        }

        public void ApplyPaging(ref IQueryable<T> query)
        {
            if (query == null) return;

            query = query.Skip(Skip).Take(Take);
        }

        #endregion

        #region Private methods

        private Expression<Func<T, bool>> GetExpressionFromFilters(CriteriaFilter[] filters)
        {
            if (filters == null) return null;

            filters = filters.Where(q => Properties.Any(w => w.Name == q.Property) && (!string.IsNullOrWhiteSpace(q.FilterValue) || !string.IsNullOrWhiteSpace(q.SecondFilterValue))).ToArray();
            if (filters.Length == 0) return null;

            try
            {
                var parameter = Expression.Parameter(typeof(T), "q");
                var expressions = filters.Select(q => GetExpressionFromFilter(parameter, q)).Where(q => q != null).ToList();
                if (!expressions.Any()) return null;

                var expression = expressions[0];
                for (var i = 1; i < expressions.Count; i++)
                {
                    expression = Expression.AndAlso(expression, expressions[i]);
                }

                return Expression.Lambda<Func<T, bool>>(expression, parameter);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private Expression GetExpressionFromFilter(ParameterExpression parameter, CriteriaFilter filter)
        {
            var pi = Properties.FirstOrDefault(q => q.Name == filter.Property);
            if (pi == null) return null;

            var expression1 = GetExpressionFromFilter(parameter, pi, filter.FilterValue, filter.FilterOperator);
            var expression2 = GetExpressionFromFilter(parameter, pi, filter.SecondFilterValue, filter.SecondFilterOperator);

            if (expression1 != null && expression2 != null)
            {
                return filter.LogicalFilterOperator == CriteriaLogicalFilterOperator.And
                    ? Expression.AndAlso(expression1, expression2)
                    : Expression.OrElse(expression1, expression2);
            }

            return expression1 ?? expression2;
        }

        private Expression GetExpressionFromFilter(ParameterExpression parameter, PropertyInfo pi, string value, CriteriaFilterOperator @operator)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;

            var typedValue = Convert.ChangeType(value, pi.PropertyType);

            return @operator switch
            {
                CriteriaFilterOperator.Equals => Expression.Equal(Expression.MakeMemberAccess(parameter, pi), Expression.Constant(typedValue, pi.PropertyType)),
                CriteriaFilterOperator.NotEquals => Expression.NotEqual(Expression.MakeMemberAccess(parameter, pi), Expression.Constant(typedValue, pi.PropertyType)),
                CriteriaFilterOperator.LessThan => Expression.LessThan(Expression.MakeMemberAccess(parameter, pi), Expression.Constant(typedValue, pi.PropertyType)),
                CriteriaFilterOperator.LessThanOrEquals => Expression.LessThanOrEqual(Expression.MakeMemberAccess(parameter, pi), Expression.Constant(typedValue, pi.PropertyType)),
                CriteriaFilterOperator.GreaterThan => Expression.GreaterThan(Expression.MakeMemberAccess(parameter, pi), Expression.Constant(typedValue, pi.PropertyType)),
                CriteriaFilterOperator.GreaterThanOrEquals => Expression.GreaterThanOrEqual(Expression.MakeMemberAccess(parameter, pi), Expression.Constant(typedValue, pi.PropertyType)),
                CriteriaFilterOperator.Contains => GetMethodExpression(parameter, pi, nameof(string.Contains), typedValue),
                CriteriaFilterOperator.StartsWith => GetMethodExpression(parameter, pi, nameof(string.StartsWith), typedValue),
                CriteriaFilterOperator.EndsWith => GetMethodExpression(parameter, pi, nameof(string.EndsWith), typedValue),

                _ => null
            };
        }

        private Expression GetMethodExpression(ParameterExpression parameter, PropertyInfo pi, string methodName, object value)
        {
            if (value == null) return null;

            var method = pi.PropertyType.GetMethod(methodName, new[] {pi.PropertyType});
            if (method == null) return null;

            var property = Expression.Property(parameter, pi);
            var valueConstant = Expression.Constant(value, pi.PropertyType);
            var methodExpression = Expression.Call(property, method, valueConstant);

            return methodExpression;
        }

        private void ApplyCriteriaSort(ref IQueryable<T> query, CriteriaSort sort)
        {
            var pi = Properties.FirstOrDefault(q => q.Name == sort.Property);
            if (pi == null) return;

            var parameter = Expression.Parameter(typeof(T), "q");
            var propertyAccess = Expression.MakeMemberAccess(parameter, pi);
            var orderExpression = Expression.Lambda(propertyAccess, parameter);

            var orderCommand = sort.SortOrder == CriteriaSortOrder.Asc ? nameof(Queryable.OrderBy) : nameof(Queryable.OrderByDescending);
            var resultExpression = Expression.Call(typeof(Queryable), orderCommand, new[] {typeof(T), pi.PropertyType}, query.Expression, Expression.Quote(orderExpression));

            query = query.Provider.CreateQuery<T>(resultExpression);
        }

        private void ApplyCriteriaSort(ref IOrderedQueryable<T> query, CriteriaSort sort)
        {
            var pi = Properties.FirstOrDefault(q => q.Name == sort.Property);
            if (pi == null) return;

            var parameter = Expression.Parameter(typeof(T), "q");
            var propertyAccess = Expression.MakeMemberAccess(parameter, pi);
            var orderExpression = Expression.Lambda(propertyAccess, parameter);

            var orderCommand = sort.SortOrder == CriteriaSortOrder.Asc ? nameof(Queryable.ThenBy) : nameof(Queryable.ThenByDescending);
            var resultExpression = Expression.Call(typeof(Queryable), orderCommand, new[] {typeof(T), pi.PropertyType}, query.Expression, Expression.Quote(orderExpression));

            query = query.Provider.CreateQuery<T>(resultExpression) as IOrderedQueryable<T>;
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
        #region C-tor | Properties

        public string Property { get; set; }

        public CriteriaSortOrder SortOrder { get; set; }

        public CriteriaSort(string property, CriteriaSortOrder sortOrder)
        {
            Property = property?.Trim();
            SortOrder = sortOrder;
        }

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
        LessThan = 2,
        LessThanOrEquals = 3,
        GreaterThan = 4,
        GreaterThanOrEquals = 5,
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