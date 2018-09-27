using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    /// <summary>
    /// ef查询扩展分页
    /// </summary>
    public static class IQueryablePageListExtensions
    {
        /// <summary>
        /// 转分页对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">查询对象</param>
        /// <param name="pageIndex">页序号</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
            => Task.FromResult(PagedList.From(source, pageIndex, pageSize));
    }
}
