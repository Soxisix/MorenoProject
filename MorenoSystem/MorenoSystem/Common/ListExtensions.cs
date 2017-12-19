using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MorenoSystem.Common
{
    public static class ListExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> coll)
        {
            var c = new ObservableCollection<T>();
            if (coll != null)
            {
                foreach (var e in coll)
                    c.Add(e);
                return c;
            }
            return null;
        }

        public static T NextOf<T>(this IList<T> list, T item)
        {
            //var nextIndex = list.IndexOf(item) + 1;

            //return nextIndex == list.Count ? list[0] : list[nextIndex];
            return list[(list.IndexOf(item) + 1) == list.Count ? list.IndexOf(list.Last()) : (list.IndexOf(item) + 1)];
        }

        public static T PreviousOf<T>(this IList<T> list, T item)
        {
            //var nextIndex = list.IndexOf(item) + 1;

            //return nextIndex == list.Count ? list[0] : list[nextIndex];
            return list[(list.IndexOf(item) - 1) == 0 ? 0 : (list.IndexOf(item) - 1)];
        }
    }


}