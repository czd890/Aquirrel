using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
namespace Aquirrel
{
    /// <summary>
    /// map扩展
    /// </summary>
    public static class MapperExtensions
    {
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDesc"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static TDesc Map<TSource, TDesc>(this TSource source)
        {
            return Microsoft.Extensions.DependencyInjection.AutoMapperConfigurationExtensions.GetMapper().Map<TSource, TDesc>(source);
        }
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDesc"></typeparam>
        /// <param name="source"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static TDesc Map<TSource, TDesc>(this TSource source, TDesc desc)
        {
            return Microsoft.Extensions.DependencyInjection.AutoMapperConfigurationExtensions.GetMapper().Map<TSource, TDesc>(source, desc);
        }
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDesc"></typeparam>
        /// <param name="sources"></param>
        /// <returns></returns>
        public static IEnumerable<TDesc> Map<TSource, TDesc>(this IEnumerable<TSource> sources)
        {
            return Microsoft.Extensions.DependencyInjection.AutoMapperConfigurationExtensions.GetMapper()
                  .Map<IEnumerable<TSource>, IEnumerable<TDesc>>(sources);
        }
        /// <summary>
        /// 转换对象
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TDesc"></typeparam>
        /// <param name="sources"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static IEnumerable<TDesc> Map<TSource, TDesc>(this IEnumerable<TSource> sources, IEnumerable<TDesc> desc)
        {
            return Microsoft.Extensions.DependencyInjection.AutoMapperConfigurationExtensions.GetMapper()
                  .Map<IEnumerable<TSource>, IEnumerable<TDesc>>(sources, desc);
        }
    }
}
