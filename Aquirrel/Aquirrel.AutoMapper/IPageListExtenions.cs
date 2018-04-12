using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel.AutoMapper
{
    public static class IPageListExtenions
    {
        public static IPagedList<TDesc> Map<TSource, TDesc>(IPagedList<TSource> source)
        {
            return source.Map(p => p.Map<TSource, TDesc>());
        }
    }
}
