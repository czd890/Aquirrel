using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquirrel
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// 扩展添加<see cref="KeyValuePair"/>对象
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="source"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add<TKey, TValue>(this IList<KeyValuePair<TKey, TValue>> source, TKey key, TValue value)
        {
            source.Add(new KeyValuePair<TKey, TValue>(key, value));
        }

        /// <summary>
        /// 使用 <paramref name="delimiter"/> 将 <paramref name="source"/> 的所有元素拼接起来
        /// </summary>
        /// <param name="source"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 循环<paramref name="source"/>对象，并使用<paramref name="each"/>处理数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="each">单项数据执行方法</param>
        public static void Each<T>(this IEnumerable<T> source, Action<T> each)
        {
            foreach (var item in source)
            {
                each(item);
            }
        }
        /// <summary>
        /// 循环<paramref name="source"/>对象，并使用<paramref name="each"/>处理数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="each">单项数据执行方法，参数一：数据，参数二：当前数据的下标</param>
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
