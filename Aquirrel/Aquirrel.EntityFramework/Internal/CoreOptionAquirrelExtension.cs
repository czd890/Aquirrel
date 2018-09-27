using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Aquirrel.EntityFramework.Internal
{
    /// <summary>
    /// ef扩展配置对象
    /// </summary>
    public class CoreOptionAquirrelExtension : IDbContextOptionsExtension
    {
        public CoreOptionAquirrelExtension()
        {

        }
        /// <summary>
        /// 创建clone一个<see cref="CoreOptionAquirrelExtension"/>对象
        /// </summary>
        /// <param name="copyFrom"></param>
        public CoreOptionAquirrelExtension(CoreOptionAquirrelExtension copyFrom)
        {
            this.EntityAssebmlys = copyFrom.EntityAssebmlys?.ToArray();
            this.EntityMappingsAssebmlys = copyFrom.EntityMappingsAssebmlys?.ToArray();
        }

        /// <summary>
        /// 自动加载的entity
        /// </summary>
        public Assembly[] EntityAssebmlys { get; set; }

        /// <summary>
        /// 自动mapping的
        /// </summary>
        public Assembly[] EntityMappingsAssebmlys { get; set; }

        public string LogFragment => "aquirrel db options";

        public bool ApplyServices(IServiceCollection services) => true;

        public long GetServiceProviderHashCode() => 0;

        public void Validate(IDbContextOptions options) { }

        public CoreOptionAquirrelExtension Clone() => new CoreOptionAquirrelExtension(this);
    }
}
