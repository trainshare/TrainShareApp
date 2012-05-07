using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;

namespace TrainShareApp.Data
{
    public static class TableExtensions
    {
        public static IEnumerable<T> UpdateOnSubmit<T>(this Table<T> table, Expression<Func<T, bool>> where,
                                                       Action<T> modify)
            where T : class
        {
            var values = table.Where(where).ToList();

            values.ForEach(modify);

            return values;
        }
    }
}