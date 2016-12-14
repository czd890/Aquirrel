using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Aquirrel.EntityFramework.Internal
{
    public class CoreOptionAquirrelExtension : IDbContextOptionsExtension
    {
        /// <summary>
        /// 自动加载的entity
        /// </summary>
        public Assembly[] EntityAssebmlys { get; set; }

        /// <summary>
        /// 自动mapping的
        /// </summary>
        public Assembly[] EntityMappingsAssebmlys { get; set; }


        public void ApplyServices(IServiceCollection services)
        {
        }

    }
}
