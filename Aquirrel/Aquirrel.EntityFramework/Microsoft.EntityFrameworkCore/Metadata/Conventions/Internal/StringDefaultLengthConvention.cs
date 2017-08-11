using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class StringDefaultLengthConvention : IPropertyAddedConvention
    {
        public InternalPropertyBuilder Apply(InternalPropertyBuilder propertyBuilder)
        {
            if (propertyBuilder.Metadata.ClrType == typeof(string))
            {
                Console.WriteLine($"default string length apply. entity:{propertyBuilder.Metadata.DeclaringType.Name}.property:{propertyBuilder.Metadata.Name}");
                propertyBuilder.HasMaxLength(32, ConfigurationSource.Convention);
            }
            return propertyBuilder;
        }
    }
}
