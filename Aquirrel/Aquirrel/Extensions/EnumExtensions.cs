using System;
using System.Collections.Generic;
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

        public static T ToEnum<T>(this int source)
        {
            return (T)Enum.ToObject(typeof(T), source);
        }

        public static string DisplayName(this Enum source)
        {
            if (source == null)
                return null;
            string description = source.ToString();
            FieldInfo fieldInfo = source.GetType().GetTypeInfo().GetField(description);
            if (fieldInfo == null)
                return null;
            DisplayAttribute[] attributes = (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
            if (attributes != null && attributes.Length > 0)
            {
                description = attributes[0].Name ?? attributes[0].Description;
            }
            return description;
        }
        enum xx { aa}
    }
}
