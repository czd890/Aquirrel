using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquirrel
{
    public static class IEnumerableExtensions
    {
        public static void Add<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> source, TKey key, TValue value)
        {
            source.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        public static void Add<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> source, params KeyValuePair<TKey, TValue>[] datas)
        {
            foreach (var item in datas)
            {
                source.Add(item);
            }
        }


        public static string ConcatEx(this IEnumerable<string> source, string delimiter)
        {
            if (source == null || source.Count() == 0)
            {
                return String.Empty;
            }
            StringBuilder sb = new StringBuilder();

            var e = source.GetEnumerator();
            e.MoveNext();

            while (true)
            {
                sb.Append(e.Current);
                if (e.MoveNext())
                {
                    sb.Append(delimiter);
                }
                else
                {
                    break;
                }
            }
            return sb.ToString();
        }

        public static void Each<T>(this IEnumerable<T> source, Action<T> each)
        {
            foreach (var item in source)
            {
                each(item);
            }
        }
        public static void Each<T>(this IEnumerable<T> source, Action<T, int> each)
        {
            int index = 0;
            foreach (var item in source)
            {
                each(item, index);
                index++;
            }
        }

    }
}
