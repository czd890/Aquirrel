using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquirrel
{
    public static class IEnumerableExtensions
    {
        public static void Add(this List<KeyValuePair<string, string>> source, string key, string value)
        {
            source.Add(new KeyValuePair<string, string>(key, value));
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

    }
}
