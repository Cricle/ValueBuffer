using System;
using System.Collections.Generic;

namespace ValueBuffer
{
    public static class ValueListFindExtensions
    {
        public static List<T> ToList<T>(this in ValueList<T> list)
        {
            var lst = new List<T>(list.Size);
            ToList(list, lst);
            return lst;
        }
        public static void ToList<T>(this in ValueList<T> list, List<T> lst)
        {
            var enu = list.GetSlotEnumerator();
            while (enu.MoveNext())
            {
                lst.AddRange(enu.CurrentArray);
            }
        }
        public static bool Contains<T>(this in ValueList<T> list, T item)
            where T : IEquatable<T>
        {
            var enu = list.GetSlotEnumerator();
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
            var totalPos = 0;
            var enu = list.GetEnumerator();
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
            var enu = list.GetSlotEnumerator();
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
