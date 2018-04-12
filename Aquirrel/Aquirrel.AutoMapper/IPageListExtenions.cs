using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel
{
    public static class PageListExtenions
    {
        public static IPagedList<TDesc> Map<TSource, TDesc>(this IPagedList<TSource> source)
        {
            return source.Map(p => p.Map<TSource, TDesc>());
        }
    }
}
