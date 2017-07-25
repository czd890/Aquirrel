using ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    public class IsMaxLengthAttributeConvention : PropertyAttributeConvention<IsMaxLengthAttribute>
    {
        public override InternalPropertyBuilder Apply(InternalPropertyBuilder propertyBuilder, IsMaxLengthAttribute attribute, MemberInfo clrMember)
        {
            propertyBuilder.IsMaxLength(attribute.IsMaxLength, ConfigurationSource.DataAnnotation);
            return propertyBuilder;
        }
    }
}
