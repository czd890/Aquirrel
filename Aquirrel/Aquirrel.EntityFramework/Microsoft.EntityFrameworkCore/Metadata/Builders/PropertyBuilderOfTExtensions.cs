using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Metadata.Builders
{
    public static class PropertyBuilderOfTExtensions
    {
        /// <summary>
        /// decimal类型设置精度
        /// </summary>
        /// <param name="propertyBuilder"></param>
        /// <param name="precision">精度</param>
        /// <param name="scale">小数位数</param>
        public static PropertyBuilder<TProperty> HasPrecision<TProperty>(this PropertyBuilder<TProperty> propertyBuilder, int precision = 18, int scale = 4)
        {
            ((IInfrastructure<InternalPropertyBuilder>)propertyBuilder).Instance.HasPrecision(precision, scale);

            return propertyBuilder;
        }

        public static PropertyBuilder<TProperty> HasMaxLength<TProperty>(this PropertyBuilder<TProperty> propertyBuilder, bool? hasMaxLength)
        {
            if (hasMaxLength.HasValue && hasMaxLength.Value == true)
            {
                Console.WriteLine($"HasMaxLength(true) remove MaxLength Annotation.entity:{propertyBuilder.Metadata.DeclaringType.Name};property:{propertyBuilder.Metadata.Name}");
                (propertyBuilder as IInfrastructure<InternalPropertyBuilder>).Instance.HasAnnotation(CoreAnnotationNames.MaxLengthAnnotation, null, ConfigurationSource.Explicit);
            }
            return propertyBuilder;
        }


       
    }
   

    public static class InternalPropertyBuilderExtensions
    {
        public static InternalPropertyBuilder HasPrecision(this InternalPropertyBuilder propertyBuilder, int precision, int scale)
        {
            propertyBuilder.Relational(ConfigurationSource.Explicit).HasColumnType($"decimal({precision},{scale})");

            return propertyBuilder;
        }
    }
}
