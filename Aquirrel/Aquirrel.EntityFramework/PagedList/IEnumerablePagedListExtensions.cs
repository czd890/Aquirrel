// Copyright (c) Arch team. All rights reserved.

using System;
using System.Collections.Generic;

namespace Aquirrel.EntityFramework
{
    /// <summary>
    /// 分页扩展
    /// </summary>
    public static class IEnumerablePagedListExtensions
    {
        /// <summary>
        /// 转分页对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">分页查询对象</param>
        /// <param name="pageIndex">页序号</param>
        /// <param name="pageSize">页大小</param>
        /// <returns></returns>
        public static IPagedList<T> ToPagedList<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
            => PagedList.From(source, pageIndex, pageSize);
    }
}
