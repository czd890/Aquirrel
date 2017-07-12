using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
   public static class PropertyBuilderOfTExtensions
    {
        public static string IsMaxLengthAnnotationName = "IsMaxLength";
        public static void IsMaxLength<TProperty>(this PropertyBuilder<TProperty> propertyBuilder, bool? ismaxLen = true)
        {
            propertyBuilder.HasAnnotation(IsMaxLengthAnnotationName, ismaxLen ?? false);
        }
    }
}
