using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
