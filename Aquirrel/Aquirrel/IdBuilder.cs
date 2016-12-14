using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel
{
    public class IdBuilderRule
    {
        /// <summary>
        /// 注册的type类型。使用AssemblyQualifiedName。如System.Object, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 字符串id前缀
        /// </summary>
        public string prefix { get; set; }
        /// <summary>
        /// 字符串id时间格式化
        /// </summary>
        public string DateFormat { get; set; }
        /// <summary>
        /// 当前id使用的ticks
        /// </summary>
        public long CurrentIdTick { get { return this._ticks; } }
        /// <summary>
        /// 精确到秒后面几位小数
        /// </summary>
        int _FCount;
        /// <summary>
        /// 生成id时间精确到秒小数后几位
        /// </summary>
        public int FCount { get { return _FCount; } set { _FCount = value; FCountMultiple = Math.Max(1, (int)Math.Pow(10, value - 1)); } }
        int FCountMultiple;
        long _ticks;
        SpinLock spinglock = new SpinLock();
        public long Next()
        {
            bool islock = false;
            spinglock.Enter(ref islock);
            if (!islock)
            {
                return this.Next();
            }

            //6361701144(秒),510(毫秒),856(微妙),00(100纳秒)
            DateTime now = DateTime.Now;
            if ((_ticks / FCountMultiple) < now.Ticks / FCountMultiple)
                _ticks = now.Ticks;
            else
                _ticks += FCountMultiple;

            spinglock.Exit();
            return _ticks;
        }
    }
    public static class IdBuilder
    {
        static IdBuilder()
        {
            _dic[typeof(IdBuilder)] = new IdBuilderRule()
            {
                prefix = "",
                FCount = 4,
                DateFormat = "yyyyMMddHHmmssffff"
            };
        }
        public static long NextId()
        {
            return NextId(typeof(IdBuilder));
        }
        public static long NextId(Type type)
        {
            var rule = _dic[type];
            if (rule == null)
                rule = _dic[typeof(IdBuilder)];

            return rule.Next();
        }
        public static string NextStringId()
        {
            return NextStringId(typeof(IdBuilder));
        }
        public static string NextStringId(Type type)
        {
            IdBuilderRule rule = null;
            if (!_dic.ContainsKey(type))
                rule = _dic[typeof(IdBuilder)];
            else
                rule = _dic[type];

            long idTick = rule.Next();
            var now = new DateTime(idTick);
            return rule.prefix + now.ToString(rule.DateFormat);
        }
        static Dictionary<Type, IdBuilderRule> _dic = new Dictionary<Type, IdBuilderRule>();
        public static void Register(Type type, string prefix, int fCount, string timeFormat)
        {
            _dic[type] = new IdBuilderRule()
            {
                prefix = prefix,
                FCount = fCount,
                DateFormat = timeFormat
            };
        }
        public static void Register(Type type, IdBuilderRule rule)
        {
            _dic[type] = rule;
        }


        static DateTime timeUtc1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        public static int TimeStampUTC()
        {
            return (DateTime.UtcNow - timeUtc1970).TotalMilliseconds.ToInt();
        }
    }
}
