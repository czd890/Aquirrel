using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal
{
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
