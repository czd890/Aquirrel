using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    /// <summary>
    /// 配置字段是否拥有最大长度
    /// </summary>
    public class HasMaxLengthAttribute : Attribute
    {
        public bool HasMaxLength { get; set; } = true;
    }
}
