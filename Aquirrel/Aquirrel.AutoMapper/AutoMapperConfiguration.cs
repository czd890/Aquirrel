using AutoMapper;
using System;

namespace Aquirrel.AutoMapper
{
    public abstract class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            this.Configure();
        }

        public abstract void Configure();
    }
}
