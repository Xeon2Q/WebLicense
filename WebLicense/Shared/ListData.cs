using System.Collections.Generic;
using System.Linq;

namespace WebLicense.Shared
{
    public sealed class ListData<T>
    {
        #region C-tor | Fields

        public int TotalCount { get; set; }

        public T[] Data { get; set; }

        private ListData(int totalCount, IEnumerable<T> data)
        {
            TotalCount = totalCount > 0 ? totalCount : 0;
            Data = data?.ToArray() ?? new T[0];
        }

        public ListData() : this(0, null) {}

        #endregion

        #region Methods

        public static ListData<TK> FromData<TK>(int totalCount, IEnumerable<TK> data)
        {
            return new(totalCount, data);
        }

        #endregion
    }
}