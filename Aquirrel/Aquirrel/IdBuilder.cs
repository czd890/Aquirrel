using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using System.Text;

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
        /// 生成id时间精确到秒小数后几位,最多6位
        /// </summary>
        public int FCount
        {
            get { return _FCount; }
            set
            {
                value = Math.Min(value, 6);
                _FCount = value;
                FCountMultiple = Math.Max(1, (int)Math.Pow(10, value - 1));
            }
        }
        int FCountMultiple;
        long _ticks;
        SpinLock spinglock = new SpinLock(false);
        public long Next()
        {
            bool islock = false;
            spinglock.Enter(ref islock);
            if (!islock) return this.Next();

            //每个计时周期表示一百纳秒，即一千万分之一秒。 1 毫秒内有 10,000 个计时周期。
            //此属性的值表示自 0001 年 1 月 1 日午夜 12:00:00（表示 DateTime.MinValue）以来经过的以 100 纳秒为间隔的间隔数。 它不包括归因于闰秒的嘀嗒数。

            //63636666581 194 153 7     ticks
            //63636666581 194 153 7 00  纳秒
            //63636666581 190 000       微妙
            //63636666581 190           毫秒
            //63636666581               秒
            DateTime now = DateTime.Now;
            if (_ticks < now.Ticks)
                _ticks = now.Ticks;
            else
            {
                //注释代码，防止机器时间同步导致无限暂停。
                //if (_ticks - now.Ticks >= 10000000)
                //    Thread.SpinWait(1000);//累加差超过了1秒,则暂停一下.
                _ticks += FCountMultiple;
            }

            spinglock.Exit(false);
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

            //long idTick = rule.Next();
            //var now = new DateTime(idTick);

            return rule.prefix + ObjectIdGenerator.NextId();
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
        public static long TimeStampUTC()
        {
            return TimeStampUTC(DateTime.UtcNow);
        }
        public static long TimeStampUTC(DateTime now)
        {
            return (long)(now.ToUniversalTime() - timeUtc1970).TotalMilliseconds;
        }

        public static int TimeStampUTCBySec()
        {
            return TimeStampUTCBySec(DateTime.UtcNow);
        }
        public static int TimeStampUTCBySec(DateTime now)
        {
            return (int)(now.ToUniversalTime() - timeUtc1970).TotalSeconds;
        }

        public static DateTime ToDateTime(long milliseconds)
        {
            return timeUtc1970.AddMilliseconds(milliseconds).ToLocalTime();
        }
        public static DateTime ToDateTime(int seconds)
        {
            return timeUtc1970.AddSeconds(seconds).ToLocalTime();
        }
    }


    public class IdWorker
    {
        private long workerId;
        private long datacenterId;
        private long sequence = 0L;

        private static long twepoch = 1288834974657L;

        private static long workerIdBits = 5L;
        private static long datacenterIdBits = 5L;
        private static long maxWorkerId = -1L ^ (-1L << (int)workerIdBits);
        private static long maxDatacenterId = -1L ^ (-1L << (int)datacenterIdBits);
        private static long sequenceBits = 12L;

        private long workerIdShift = sequenceBits;
        private long datacenterIdShift = sequenceBits + workerIdBits;
        private long timestampLeftShift = sequenceBits + workerIdBits + datacenterIdBits;
        private long sequenceMask = -1L ^ (-1L << (int)sequenceBits);

        private long lastTimestamp = -1L;
        private static object syncRoot = new object();

        public IdWorker(long workerId, long datacenterId)
        {

            // sanity check for workerId
            if (workerId > maxWorkerId || workerId < 0)
            {
                throw new ArgumentException(string.Format("worker Id can't be greater than %d or less than 0", maxWorkerId));
            }
            if (datacenterId > maxDatacenterId || datacenterId < 0)
            {
                throw new ArgumentException(string.Format("datacenter Id can't be greater than %d or less than 0", maxDatacenterId));
            }
            this.workerId = workerId;
            this.datacenterId = datacenterId;
        }

        public long nextId()
        {
            lock (syncRoot)
            {
                long timestamp = timeGen();

                if (timestamp < lastTimestamp)
                {
                    throw new ApplicationException(string.Format("Clock moved backwards.  Refusing to generate id for %d milliseconds", lastTimestamp - timestamp));
                }

                if (lastTimestamp == timestamp)
                {
                    sequence = (sequence + 1) & sequenceMask;
                    if (sequence == 0)
                    {
                        timestamp = tilNextMillis(lastTimestamp);
                    }
                }
                else
                {
                    sequence = 0L;
                }

                lastTimestamp = timestamp;

                return ((timestamp - twepoch) << (int)timestampLeftShift) | (datacenterId << (int)datacenterIdShift) | (workerId << (int)workerIdShift) | sequence;
            }
        }

        protected long tilNextMillis(long lastTimestamp)
        {
            long timestamp = timeGen();
            while (timestamp <= lastTimestamp)
            {
                timestamp = timeGen();
            }
            return timestamp;
        }

        protected long timeGen()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }

    public class ObjectId
    {
        private string _string;

        public ObjectId()
        {
        }

        public ObjectId(string value)
          : this(DecodeHex(value))
        {
        }

        internal ObjectId(byte[] value)
        {
            Value = value;
        }

        public static ObjectId Empty
        {
            get { return new ObjectId("000000000000000000000000"); }
        }

        public byte[] Value { get; private set; }

        public static ObjectId NewObjectId()
        {
            return new ObjectId { Value = ObjectIdGenerator.Generate() };
        }

        public static bool TryParse(string value, out ObjectId objectId)
        {
            objectId = Empty;
            if (value == null || value.Length != 24)
            {
                return false;
            }

            try
            {
                objectId = new ObjectId(value);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        protected static byte[] DecodeHex(string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException("value");

            var chars = value.ToCharArray();
            var numberChars = chars.Length;
            var bytes = new byte[numberChars / 2];

            for (var i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(new string(chars, i, 2), 16);
            }
            return bytes;
        }

        public override int GetHashCode()
        {
            return Value != null ? ToString().GetHashCode() : 0;
        }

        public override string ToString()
        {
            if (_string == null && Value != null)
            {
                _string = BitConverter.ToString(Value)
                  .Replace("-", string.Empty)
                  .ToLowerInvariant();
            }

            return _string;
        }

        public override bool Equals(object obj)
        {
            var other = obj as ObjectId;
            return Equals(other);
        }

        public bool Equals(ObjectId other)
        {
            return other != null && ToString() == other.ToString();
        }

        public static implicit operator string(ObjectId objectId)
        {
            return objectId == null ? null : objectId.ToString();
        }

        public static implicit operator ObjectId(string value)
        {
            return new ObjectId(value);
        }

        public static bool operator ==(ObjectId left, ObjectId right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(ObjectId left, ObjectId right)
        {
            return !(left == right);
        }

        public DateTime TimeStamp => getTimeStamp();
        public int Counter => getCounter();
        public string Ip => getIp();
        public int Pid => getPid();

        int getPid()
        {
            return BitConverter.ToInt32(new byte[4] { this.Value[11], this.Value[10], 0, 0 }, 0);
        }
        int getCounter()
        {
            return BitConverter.ToInt32(new byte[4] { this.Value[5], this.Value[4], 0, 0 }, 0);
        }
        string getIp()
        {
            return $"{this.Value[9]}.{this.Value[8]}.{this.Value[7]}.{this.Value[6]}";
        }
        DateTime getTimeStamp()
        {
            var sec = BitConverter.ToInt32(new byte[4] { this.Value[3], this.Value[2], this.Value[1], this.Value[0], }, 0);
            return IdBuilder.ToDateTime(sec);
        }
    }

    internal static class ObjectIdGenerator
    {
        static object _innerLock = new object();
        static byte[] _ip = getIp().Reverse().ToArray() ?? new byte[4];
        static byte[] _processId = BitConverter.GetBytes(GenerateProcessId()).Reverse().ToArray();

        public static string NextId() => BitConverter.ToString(Generate()).Replace("-", "").ToLower();
        public static byte[] Generate()
        {
            //4字节时间|2字节自增|4字节ip|2字节pid
            var oid = new byte[12];
            var sec = IdBuilder.TimeStampUTCBySec();
            Array.Copy(BitConverter.GetBytes(sec).Reverse().ToArray(), 0, oid, 0, 4);

            var counter = GenerateCounter(sec);
            Array.Copy(BitConverter.GetBytes(counter).Reverse().ToArray(), 2, oid, 4, 2);
            Array.Copy(_ip, 0, oid, 6, 4);
            Array.Copy(_processId, 2, oid, 10, 2);

            return oid;
        }

        static int GenerateProcessId()
        {
            var process = Process.GetCurrentProcess();
            return process.Id;
        }

        static byte[] getIp()
        {

            return System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                     .Select(p => p.GetIPProperties())
                     .SelectMany(p => p.UnicastAddresses)
                     .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                     .FirstOrDefault()?.Address.GetAddressBytes();
        }

        static int _last_sec;
        static int _counter;
        static int GenerateCounter(int current_sec)
        {
            //Interlocked.Increment(ref _counter);
            lock (_innerLock)
            {

                if (current_sec > _last_sec)
                    _counter = 0;

                _counter++;
                _last_sec = current_sec;
                return _counter;
            }
        }
    }
}
