using System;
using System.Collections.Generic;
using System.Text;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Internal;

namespace Microsoft.EntityFrameworkCore.Infrastructure.Internal
{
    public class MyRelationalModelSource : RelationalModelSource
    {
        public MyRelationalModelSource(ModelSourceDependencies dependencies) : base(dependencies)
        {
        }
        protected override IModel CreateModel(DbContext context, IConventionSetBuilder conventionSetBuilder, IModelValidator validator)
        {
            return base.CreateModel(context, conventionSetBuilder, validator);
        }
        public override IModel GetModel(DbContext context, IConventionSetBuilder conventionSetBuilder, IModelValidator validator)
        {
            return base.GetModel(context, conventionSetBuilder, validator);
        }
    }
}
