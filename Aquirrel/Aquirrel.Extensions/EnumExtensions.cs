using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aquirrel
{
    public static class EnumExtensions
    {
        /// <summary>
        /// 枚举转int类型
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static int ToInt(this Enum source)
        {
            return ((IConvertible)source).ToInt32(null);
        }
        /// <summary>
        /// int类型转枚举
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T ToEnum<T>(this int source) => (T)Enum.ToObject(typeof(T), source);
        /// <summary>
        /// 获得枚举使用<see cref="DescriptionAttribute"/>或<see cref="DisplayAttribute"/>或<see cref="DisplayNameAttribute"/>修饰的显示名称
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string DisplayName(this Enum source)
        {

            if (source == null)
                return null;
            string description = source.ToString();
            FieldInfo fieldInfo = source.GetType().GetTypeInfo().GetField(description);
            if (fieldInfo == null)
                return null;

            DescriptionAttribute[] attributes2 = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes2 != null && attributes2.Length > 0)
            {
                return attributes2[0].Description ?? description;
            }

            DisplayAttribute[] attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Name ?? attributes[0].Description;
            }
            DisplayNameAttribute[] attributes3 = (DisplayNameAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            if (attributes3 != null && attributes3.Length > 0)
            {
                return attributes3[0].DisplayName ?? description;
            }

            return description;
        }
        /// <summary>
        /// 当前枚举值是否在枚举中被定义
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDefined(this Enum source) => Enum.IsDefined(source.GetType(), source);
    }
}
