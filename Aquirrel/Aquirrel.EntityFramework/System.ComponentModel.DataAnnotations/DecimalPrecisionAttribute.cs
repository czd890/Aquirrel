using System;
using System.Collections.Generic;
using System.Text;

namespace System.ComponentModel.DataAnnotations
{
    public class DecimalPrecisionAttribute : Attribute
    {
        /// <summary>
        /// 精度
        /// </summary>
        public int Precision { get; set; } = 18;
        /// <summary>
        /// 小数位数
        /// </summary>
        public int Scale { get; set; } = 4;


    }
}
