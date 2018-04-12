using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework
{
    public static class IQueryablePageListExtensions
    {
        public static Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
            => Task.FromResult(PagedList.From(source, pageIndex, pageSize));
    }
}
