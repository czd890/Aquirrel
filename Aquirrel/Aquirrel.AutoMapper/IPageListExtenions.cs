using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel
{
    public static class PageListExtenions
    {
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDesc"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static IPagedList<TDesc> Map<TSource, TDesc>(this IPagedList<TSource> source)
        {
            return source.Map(p => p.Map<TSource, TDesc>());
        }
    }
}
