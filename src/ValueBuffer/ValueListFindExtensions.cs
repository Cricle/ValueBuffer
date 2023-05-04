using System;
using System.Collections.Generic;

namespace ValueBuffer
{
    public static class ValueListFindExtensions
    {
        public static List<T> ToList<T>(this in ValueList<T> list)
        {
            var lst = new List<T>(list.Size);
            ToList(in list, lst);
            return lst;
        }
        public static void ToList<T>(this in ValueList<T> list, List<T> lst)
        {
            if (lst is null)
            {
                throw new ArgumentNullException(nameof(lst));
            }

            var enu = ValueList<T>.GetSlotEnumerator(in list);
            while (enu.MoveNext())
            {
                lst.AddRange(enu.CurrentArray);
            }
        }
        public static bool Contains<T>(this in ValueList<T> list, T item)
            where T : IEquatable<T>
        {
            var enu = ValueList<T>.GetSlotEnumerator(in list);
            while (enu.MoveNext())
            {
                var pos = enu.Current.IndexOf(item);
                if (pos != -1)
                {
                    return true;
                }
            }
            return false;
        }
        public static int FindIndex<T>(this in ValueList<T> list, Predicate<T> condition)
           where T : IEquatable<T>
        {
            if (condition is null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var totalPos = 0;
            var enu = ValueList<T>.GetEnumerator(in list);
            while (enu.MoveNext())
            {
                if (condition(enu.Current))
                {
                    return totalPos;
                }
                totalPos++;
            }
            return -1;
        }
        public static int FindIndex<T>(this in ValueList<T> list, T item)
            where T : IEquatable<T>
        {
            var totalPos = 0;
            var enu = ValueList<T>.GetSlotEnumerator(in list);
            while (enu.MoveNext())
            {
                var pos = enu.Current.IndexOf(item);
                if (pos != -1)
                {
                    return totalPos + pos;
                }
                totalPos += enu.Current.Length;
            }
            return -1;
        }
    }
}
