using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Aquirrel
{
    public static class StringExtensions
    {
        public static string FormatEx(this string source, params object[] items)
        {
            return String.Format(source, items);
        }
        public static bool ContainsEx(this string source, string value)
        {
            if (String.IsNullOrEmpty(source))
                return false;
            return source.Contains(value);
        }
        public static int ToInt(this string source, int defaultValue = 0)
        {
            var r = 0;
            if (!int.TryParse(source, out r))
                r = defaultValue;
            return r;
        }

        public static decimal ToDecimal(this string source, decimal defaultValue = 0)
        {
            var r = 0m;
            if (!decimal.TryParse(source, out r))
                r = defaultValue;
            return r;
        }

        public static bool IsNullOrEmpty(this string source)
        {
            return String.IsNullOrEmpty(source);
        }
        public static bool IsNotNullOrEmpty(this string source)
        {
            return !String.IsNullOrEmpty(source);
        }

        public static T ToJson<T>(this string source)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(source);
        }

        public static DateTime ToDateTime(this string source)
        {
            DateTime date;
            DateTime.TryParse(source, out date);
            return date;
        }
        public static DateTime? ToDBDateTime(this string source)
        {
            DateTime date;
            DateTime.TryParse(source, out date);
            if (date < new DateTime(1970, 1, 1))
                return null;
            return date;
        }

        public static T ToEnum<T>(this string source)
        {
            return (T)Enum.Parse(typeof(T), source);
        }

        public static T ToXml<T>(this string xml, Encoding encoding = null)
        {
            if (xml.IsNullOrEmpty())
                return default(T);
            if (encoding == null)
                encoding = Encoding.UTF8;
            XmlSerializer ser = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(encoding.GetBytes(xml)))
            {
                using (StreamReader sr = new StreamReader(ms, encoding))
                {
                    return (T)ser.Deserialize(sr);
                }
            }
        }

        /// <summary>
        /// 转化为半角字符串
        /// </summary>
        /// <param name="input">要转化的字符串</param>
        /// <returns>半角字符串</returns>
        public static string ToSBC(this string input)//single byte charactor
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)//全角空格为12288，半角空格为32
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)//其他字符半角(33-126)与全角(65281-65374)的对应关系是：均相差65248
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// 转化为全角字符串
        /// </summary>
        /// <param name="input">要转化的字符串</param>
        /// <returns>全角字符串</returns>
        public static string ToDBC(this string input)//double byte charactor 
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }

        public static string ToBase64(this string source, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return Convert.ToBase64String(encoding.GetBytes(source));
        }

    }
}
