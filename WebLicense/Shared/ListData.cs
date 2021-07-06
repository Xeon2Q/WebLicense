using System.Collections.Generic;
using System.Linq;

namespace WebLicense.Shared
{
    public sealed class ListData<T>
    {
        #region C-tor | Properties

        public int Total { get; }

        public int TotalFiltered { get; }

        public IReadOnlyCollection<T> Data { get; }

        public ListData() : this(0, 0, null)
        {
        }

        public ListData(int total, int totalFiltered, IEnumerable<T> data)
        {
            Total = total > 0 ? total : 0;
            TotalFiltered = totalFiltered > 0 ? totalFiltered : 0;
            Data = data?.ToArray() ?? new T[0];
        }

        #endregion
    }
}