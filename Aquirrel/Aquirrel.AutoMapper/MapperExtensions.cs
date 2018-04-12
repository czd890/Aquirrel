using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
namespace Aquirrel
{
    public static class MapperExtensions
    {
        public static TDesc Map<TSource, TDesc>(this TSource source)
        {
            return Microsoft.Extensions.DependencyInjection.AutoMapperConfigurationExtensions.GetMapper().Map<TSource, TDesc>(source);
        }
        public static TDesc Map<TSource, TDesc>(this TSource source, TDesc desc)
        {
            return Microsoft.Extensions.DependencyInjection.AutoMapperConfigurationExtensions.GetMapper().Map<TSource, TDesc>(source, desc);
        }

        public static IEnumerable<TDesc> Map<TSource, TDesc>(this IEnumerable<TSource> sources)
        {
            return Microsoft.Extensions.DependencyInjection.AutoMapperConfigurationExtensions.GetMapper()
                  .Map<IEnumerable<TSource>, IEnumerable<TDesc>>(sources);
        }
        public static IEnumerable<TDesc> Map<TSource, TDesc>(this IEnumerable<TSource> sources, IEnumerable<TDesc> desc)
        {
            return Microsoft.Extensions.DependencyInjection.AutoMapperConfigurationExtensions.GetMapper()
                  .Map<IEnumerable<TSource>, IEnumerable<TDesc>>(sources, desc);
        }
    }
}
