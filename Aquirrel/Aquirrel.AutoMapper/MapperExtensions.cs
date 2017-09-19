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
            return Microsoft.Extensions.DependencyInjection.AutoMapperConfigurationExtensions._mapper.Map<TSource, TDesc>(source);
        }
        public static TDesc Map<TSource, TDesc>(this TSource source, TDesc desc)
        {
            return Microsoft.Extensions.DependencyInjection.AutoMapperConfigurationExtensions._mapper.Map<TSource, TDesc>(source, desc);
        }
    }
}
