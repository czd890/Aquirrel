using System;
using System.Collections.Generic;
using System.Threading;
using Aquirrel.ResetApi.Internal;

namespace Aquirrel.Tracing.Internal
{
    public class RequestEntry : IRequestEntry


    {
        /// <summary>
        /// 异步线程共享变量
        /// </summary>
        public static AsyncLocal<IRequestEntry> ALS = new AsyncLocal<IRequestEntry>(xx =>
        {
            Console.WriteLine("--");
        });

        public RequestEntry()
        {
            this.ChildRequest = new Dictionary<string, IRequestEntry>();
            this.Datas = new Dictionary<string, object>();
            this.Id = Guid.NewGuid().ToString("N");
        }
        /// <summary>
        /// 当前应用名字
        /// </summary>
        public string App { get; set; }

        public string TraceId { get; set; }
        public string TraceDepth { get; set; }
        public string LocalIp { get; set; }
        public string ClientIp { get; set; }
        public DateTime BeginTime { get; set; }

        public Dictionary<string, IRequestEntry> ChildRequest { get; private set; }
        public DateTime? EndTime { get; set; }

        public Dictionary<string, object> Datas { get; private set; }

        public int Duration => this.EndTime.HasValue ? (this.EndTime > this.BeginTime ? ((int)(this.EndTime.Value - this.BeginTime).TotalMilliseconds) : 0) : 0;

        public Exception Exception { get; set; }
        public string UserOpenId { get; set; }
        public string UserTraceId { get; set; }
        public string AccessToken { get; set; }
        public string RealIp { get; set; }

        private int _currentDepth;

        public string Id { get; }


        public RequestEntry Clone()
        {
            return new RequestEntry()
            {
                App = this.App,
                TraceId = this.TraceId,
                TraceDepth = this.TraceDepth,
                LocalIp = this.LocalIp,
                ClientIp = this.ClientIp,
                BeginTime = this.BeginTime,
                EndTime = this.EndTime
            };
        }
        public IRequestEntry NewChildRequest()
        {
            var childReq = this.Clone();
            if (_currentDepth == 0)
                _currentDepth = RestApiConst.NewDepth();
            else
                _currentDepth += 1;
            childReq.TraceDepth = this.TraceDepth + _currentDepth.ToString();
            this.ChildRequest.Add(childReq.TraceDepth, childReq);
            return childReq;
        }
    }
}
