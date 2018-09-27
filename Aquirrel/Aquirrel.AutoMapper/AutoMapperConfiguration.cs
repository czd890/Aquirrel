using AutoMapper;
using System;

namespace Aquirrel.AutoMapper
{
    /// <summary>
    /// 配置map对象转换
    /// </summary>
    public abstract class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            this.Configure();
        }

        public abstract void Configure();
    }
}
