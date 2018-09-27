using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    /// <summary>
    /// 解析<see cref="DecimalPrecisionAttribute"/>标记的字段
    /// </summary>
    public class DecimalPrecisionAttributeConvention : PropertyAttributeConvention<DecimalPrecisionAttribute>
    {
        public override InternalPropertyBuilder Apply(InternalPropertyBuilder propertyBuilder, DecimalPrecisionAttribute attribute, MemberInfo clrMember)
        {
            if (propertyBuilder.Metadata.ClrType == typeof(decimal))
                propertyBuilder.HasPrecision(ConfigurationSource.DataAnnotation, attribute.Precision, attribute.Scale);
            return propertyBuilder;
        }
    }
}
