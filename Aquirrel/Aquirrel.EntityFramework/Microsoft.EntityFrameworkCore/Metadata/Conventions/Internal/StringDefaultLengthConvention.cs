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
                propertyBuilder.HasMaxLength(32, ConfigurationSource.Convention);
            return propertyBuilder;
        }
    }
}
