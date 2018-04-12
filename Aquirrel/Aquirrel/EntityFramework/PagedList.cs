using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Aquirrel;
namespace Aquirrel
{
    /// <summary>
    /// Represents the default implementation of the <see cref="IPagedList{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the data to page</typeparam>
    internal class PagedList<T> : IPagedList<T>
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

        bool hasGetItems;
        IEnumerable<T> orign_Items;
        IEnumerable<T> curr_Items;
        /// <summary>
        /// Gets or sets the items.
        /// </summary>
        /// <value>The items.</value>
        public IEnumerable<T> Items
        {
            get
            {
                if (hasGetItems && curr_Items != null)
                    return curr_Items;

                curr_Items = orign_Items.ToArray();
                hasGetItems = true;
                return curr_Items;
            }
            set
            {
                curr_Items = null;
                hasGetItems = false;
                orign_Items = value;
            }
        }

        /// <summary>
        /// Gets the has previous page.
        /// </summary>
        /// <value>The has previous page.</value>
        public bool HasPreviousPage => PageIndex > 0;

        /// <summary>
        /// Gets the has next page.
        /// </summary>
        /// <value>The has next page.</value>
        public bool HasNextPage => PageIndex + 1 < TotalPages;

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}" /> class.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="pageIndex">The index of the page.</param>
        /// <param name="pageSize">The size of the page.</param>
        /// <param name="indexFrom">The index from.</param>
        internal PagedList(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var _s = source.AsQueryable();
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = _s.Count();

            Items = _s.Skip(PageIndex * PageSize).Take(PageSize);
        }
        internal PagedList(IEnumerable<T> items, int pageIndex, int pageSize, int totalCount)
        {
            this.Items = items;
            this.PageIndex = pageIndex;
            this.PageSize = PageSize;
            this.TotalCount = totalCount;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PagedList{T}" /> class.
        /// </summary>
        internal PagedList() => Items = Array.Empty<T>();

        //public IPagedList<TDesc> Map<TDesc>()
        //{
        //    return new PagedList<T, TDesc>(this);
        //}

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
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IEnumerable<TResult> Items { get; set; }

        /// <summary>
        /// Gets the has previous page.
        /// </summary>
        /// <value>The has previous page.</value>
        public bool HasPreviousPage => PageIndex > 0;

        /// <summary>
        /// Gets the has next page.
        /// </summary>
        /// <value>The has next page.</value>
        public bool HasNextPage => PageIndex + 1 < TotalPages;

        public PagedList(IPagedList<TSource> source, Func<TSource, TResult> converter)
        {
            PageIndex = source.PageIndex;
            PageSize = source.PageSize;
            TotalCount = source.TotalCount;
            TotalPages = source.TotalPages;

            Items = new CachedMapEnumerable<TSource, TResult>(source.Items, converter);
        }

        public IPagedList<TDesc> Map<TDesc>(Func<TResult, TDesc> converter)
        {
            return new PagedList<TResult, TDesc>(this, converter);
        }
    }

    public static class PagedList
    {
        public static IPagedList<T> From<T>(IEnumerable<T> items, int pageIndex, int pageSize, int totalCount)
        {
            return new PagedList<T>(items, pageIndex, pageSize, totalCount);
        }

        public static IPagedList<T> From<T>(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            return new PagedList<T>(source, pageIndex, pageSize);
        }
    }
}
