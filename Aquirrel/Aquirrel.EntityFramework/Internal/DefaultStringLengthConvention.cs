using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.EntityFramework.Internal
{
    //public sealed class DefaultStringLengthConvention : IModelConvention
    //{
    //    internal const int DefaultStringLength = 32;
    //    internal const string MaxLengthAnnotation = "MaxLength";

    //    private readonly int _defaultStringLength;

    //    public DefaultStringLengthConvention(int defaultStringLength = DefaultStringLength)
    //    {
    //        this._defaultStringLength = defaultStringLength;
            
    //    }

    //    public InternalModelBuilder Apply(InternalModelBuilder modelBuilder)
    //    {
    //        Console.WriteLine("DefaultStringLengthConvention.Apply");
    //        foreach (var entity in modelBuilder.Metadata.GetEntityTypes())
    //        {
    //            foreach (var property in entity.GetProperties())
    //            {
    //                if (property.ClrType == typeof(string))
    //                {
    //                    if (property.FindAnnotation(MaxLengthAnnotation) == null)
    //                    {
    //                        property.AddAnnotation(MaxLengthAnnotation, this._defaultStringLength);
    //                    }
    //                }
    //            }
    //        }

    //        return modelBuilder;
    //    }
    //}

}
