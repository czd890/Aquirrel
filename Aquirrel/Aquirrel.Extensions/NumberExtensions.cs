using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquirrel
{
    public static class NumberExtensions
    {
        public static int ToInt(this decimal number)
        {
            return (int)number;
        }
        public static int ToInt(this double number)
        {
            return (int)number;
        }
        public static int ToInt(this long number)
        {
            return (int)number;
        }
        public static int ToInt(this float number)
        {
            return (int)number;
        }


        public static long ToLong(this double number)
        {
            return (long)number;
        }
        public static long ToLong(this decimal number)
        {
            return (long)number;
        }
        public static long ToLong(this float number)
        {
            return (long)number;
        }


        public static decimal ToDecimal(this double number)
        {
            return (decimal)number;
        }
        public static decimal ToDecimal(this float number)
        {
            return (decimal)number;
        }

        public static double ToDecimal(this decimal number)
        {
            return (double)number;
        }

        public static float ToFloat(this decimal number)
        {
            return (float)number;
        }
        public static float ToFloat(this double number)
        {
            return (float)number;
        }
        /// <summary>
        /// source是否在min和max中，包含min和max。min&gt;=source&lt;=max
        /// </summary>
        /// <typeparam name="T">值类型或实现了IComparable接口的成员</typeparam>
        /// <param name="source"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static bool In<T>(this T source, T min, T max) where T : IComparable<T>
        {
            //个值，指示要比较的对象的相对顺序。 返回值的含义如下： 值 含义 小于零 此对象小于 other 参数。 零 此对象等于 other。 大于零 此对象大于
            var s = source as IComparable<T>;
            return s.CompareTo(min) >= 0 && s.CompareTo(max) <= 0;
        }
    }
}
