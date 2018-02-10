using System;
using System.Collections.Generic;
using System.Linq;
using Aquirrel;
namespace Aquirrel.EntityFramework
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IPagedList{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the data to page</typeparam>
    public class PagedList<T> : IPagedList<T>
    {
        /// <summary>
        /// Gets or sets the index of the page.
        /// </summary>
        /// <value>The index of the page.</value>
        public int PageIndex { get; set; }
        /// <summary>
        /// Gets or sets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; set; }
        /// <summary>
        /// Gets or sets the total count.
        /// </summary>
        /// <value>The total count.</value>
        public int TotalCount { get; set; }
        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        /// <value>The total pages.</value>
        public int TotalPages => (int)Math.Ceiling(this.TotalCount / (double)this.PageSize);
        /// <summary>
        /// Gets or sets the index from.
        /// </summary>
        /// <value>The index from.</value>
        public int IndexFrom { get; set; }

        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        /// Gets the has previous page.
        /// </summary>
        /// <value>The has previous page.</value>
        public bool HasPreviousPage => PageIndex - IndexFrom > 0;

        /// <summary>
        /// Gets the has next page.
        /// </summary>
        /// <value>The has next page.</value>
        public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="indexFrom">The index from.</param>
        internal PagedList(IEnumerable<T> source, int pageIndex, int pageSize, int indexFrom)
        {
            if (indexFrom > pageIndex)
            {
                throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
            }
            var _s = source.AsQueryable();
            PageIndex = pageIndex;
            PageSize = pageSize;
            IndexFrom = indexFrom;
            TotalCount = _s.Count();

            Items = _s.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}" /> class.
        /// </summary>
        internal PagedList() => Items = Array.Empty<T>();

        public IPagedList<TDesc> Map<TDesc>()
        {
            return new PagedList<T, TDesc>(this);
        }

        public IPagedList<TDesc> Map<TDesc>(Func<T, TDesc> converter)
        {
            return new PagedList<T, TDesc>(this, converter);
        }
    }


    /// <summary>
    /// Provides the implementation of the <see cref="IPagedList{T}"/> and converter.
    /// </summary>
    /// <typeparam name="TSource">The type of the source.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    internal class PagedList<TSource, TResult> : IPagedList<TResult>
    {
        /// <summary>
        /// Gets the index of the page.
        /// </summary>
        /// <value>The index of the page.</value>
        public int PageIndex { get; }
        /// <summary>
        /// Gets the size of the page.
        /// </summary>
        /// <value>The size of the page.</value>
        public int PageSize { get; }
        /// <summary>
        /// Gets the total count.
        /// </summary>
        /// <value>The total count.</value>
        public int TotalCount { get; }
        /// <summary>
        /// Gets the total pages.
        /// </summary>
        /// <value>The total pages.</value>
        public int TotalPages { get; }
        /// <summary>
        /// Gets the index from.
        /// </summary>
        /// <value>The index from.</value>
        public int IndexFrom { get; }

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IEnumerable<TResult> Items { get; }

        /// <summary>
        /// Gets the has previous page.
        /// </summary>
        /// <value>The has previous page.</value>
        public bool HasPreviousPage => PageIndex - IndexFrom > 0;

        /// <summary>
        /// Gets the has next page.
        /// </summary>
        /// <value>The has next page.</value>
        public bool HasNextPage => PageIndex - IndexFrom + 1 < TotalPages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{TSource, TResult}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="converter">The converter.</param>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="indexFrom">The index from.</param>
        //public PagedList(IEnumerable<TSource> source, Func<IEnumerable<TSource>, IEnumerable<TResult>> converter, int pageIndex, int pageSize, int indexFrom)
        //{
        //    if (indexFrom > pageIndex)
        //    {
        //        throw new ArgumentException($"indexFrom: {indexFrom} > pageIndex: {pageIndex}, must indexFrom <= pageIndex");
        //    }

        //    PageIndex = pageIndex;
        //    PageSize = pageSize;
        //    IndexFrom = indexFrom;
        //    TotalCount = source.Count();
        //    TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);

        //    var items = source.Skip((PageIndex - IndexFrom) * PageSize).Take(PageSize).ToArray();

        //    Items = new List<TResult>(converter(items));
        //}

        public PagedList(IPagedList<TSource> source)
        {
            PageIndex = source.PageIndex;
            PageSize = source.PageSize;
            IndexFrom = source.IndexFrom;
            TotalCount = source.TotalCount;
            TotalPages = source.TotalPages;

            Items = Converter(source.Items);
        }

        public PagedList(IPagedList<TSource> source, Func<TSource, TResult> converter)
        {
            PageIndex = source.PageIndex;
            PageSize = source.PageSize;
            IndexFrom = source.IndexFrom;
            TotalCount = source.TotalCount;
            TotalPages = source.TotalPages;

            Items = Converter(source.Items, converter);
        }
        IEnumerable<TResult> Converter(IEnumerable<TSource> _source)
        {
            Console.WriteLine("Converter");
            foreach (var item in _source)
            {
                Console.WriteLine("Converter item");
                yield return item.Map<TSource, TResult>();
            }
        }
        IEnumerable<TResult> Converter(IEnumerable<TSource> _source, Func<TSource, TResult> converter)
        {
            foreach (var item in _source)
            {
                yield return converter(item);
            }
        }

        public IPagedList<TDesc> Map<TDesc>()
        {
            return new PagedList<TResult, TDesc>(this);
        }

        public IPagedList<TDesc> Map<TDesc>(Func<TResult, TDesc> converter)
        {
            return new PagedList<TResult, TDesc>(this, converter);
        }
    }

    /// <summary>
    /// Provides some help methods for <see cref="IPagedList{T}"/> interface.
    /// </summary>
    public static class PagedList
    {
        /// <summary>
        /// Creates an empty of <see cref="IPagedList{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type for paging </typeparam>
        /// <returns>An empty instance of <see cref="IPagedList{T}"/>.</returns>
        //public static IPagedList<T> Empty<T>() => new PagedList<T>();
        /// <summary>
        /// Creates a new instance of <see cref="IPagedList{TResult}"/> from source of <see cref="IPagedList{TSource}"/> instance.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <typeparam name="TSource">The type of the source.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="converter">The converter.</param>
        /// <returns>An instance of <see cref="IPagedList{TResult}"/>.</returns>
        //public static IPagedList<TResult> From<TResult, TSource>(this IPagedList<TSource> source, Func<TSource, TResult> converter) => new PagedList<TSource, TResult>(source, converter);
    }
}
