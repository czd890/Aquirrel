using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    public class IsMaxLengthAttribute : Attribute
    {
        public bool IsMaxLength { get; set; } = true;
    }
}
