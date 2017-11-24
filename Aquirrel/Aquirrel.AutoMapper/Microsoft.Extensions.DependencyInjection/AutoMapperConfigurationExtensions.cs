using Aquirrel.AutoMapper;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{

    public static class AutoMapperConfigurationExtensions
    {
        static List<Type> _profile = new List<Type>();
        static IMapper _mapper;

        public static IServiceCollection AddAutoMapperProfile<IProfile>(this IServiceCollection services)
            where IProfile : AutoMapperConfiguration, new()
        {
            _profile.Add(typeof(IProfile));

            return services;
        }
        public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
        {

            services.AddSingleton<IMapper>(sp =>
            {
                return GetMapper();
            });

            return services;
        }

        public static IMapper GetMapper()
        {
            if (_mapper != null)
                return _mapper;

            var _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                foreach (var item in _profile)
                {
                    cfg.AddProfile(item);
                }
            });
            return _mapper = _mapperConfiguration.CreateMapper();
        }
    }
}
