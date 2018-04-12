// Copyright (c) Arch team. All rights reserved.

using System;
using System.Collections.Generic;

namespace Aquirrel.EntityFramework
{
    public static class IEnumerablePagedListExtensions
    {
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
            => PagedList.From(source, pageIndex, pageSize);
    }
}
