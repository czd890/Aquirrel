using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    public class HasMaxLengthAttribute : Attribute
    {
        public bool HasMaxLength { get; set; } = true;
    }
}
