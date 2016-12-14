using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquirrel
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// 格式化时间为字符串格式
        /// </summary>
        public static string TextForDate(this DateTime date, string format = "yyyy/MM/dd")
        {
            return date.ToString(format);
        }
        /// <summary>
        /// 获取该日期中的时间部分 HH:mm:ss.ttt
        /// </summary>
        public static TimeSpan ToTime(this DateTime date)
        {
            return new TimeSpan(0, date.Hour, date.Minute, date.Second, date.Millisecond);
        }
    }
}
