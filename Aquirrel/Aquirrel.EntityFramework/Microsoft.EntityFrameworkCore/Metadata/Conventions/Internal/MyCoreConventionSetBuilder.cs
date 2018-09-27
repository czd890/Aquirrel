using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
    /// <summary>
    /// 扩展ef核心配置对象，增加默认字段约束（字符串默认32长度，decimal默认18/4精度，字符串默认最大程度）
    /// </summary>
    public class MyCoreConventionSetBuilder : CoreConventionSetBuilder
    {
        public MyCoreConventionSetBuilder(CoreConventionSetBuilderDependencies dependencies) : base(dependencies)
        {
            Console.WriteLine("MyRelationalModelCustomizer ctor");
        }
        public override ConventionSet CreateConventionSet()
        {
            Console.WriteLine("MyCoreConventionSetBuilder.CreateConventionSet");

            var conventionSet = base.CreateConventionSet();

            conventionSet.PropertyAddedConventions.Insert(0, new StringDefaultLengthConvention());
            conventionSet.PropertyAddedConventions.Add(new DecimalPrecisionAttributeConvention());
            conventionSet.PropertyAddedConventions.Add(new HasMaxLengthAttributeConvention());

            return conventionSet;
        }
    }
}
