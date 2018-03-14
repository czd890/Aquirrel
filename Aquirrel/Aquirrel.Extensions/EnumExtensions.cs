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
        public static int ToInt(this Enum source)
        {
            return ((IConvertible)source).ToInt32(null);
        }

        public static T ToEnum<T>(this int source) => (T)Enum.ToObject(typeof(T), source);

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

        public static bool IsDefined(this Enum source) => Enum.IsDefined(source.GetType(), source);
    }
}
