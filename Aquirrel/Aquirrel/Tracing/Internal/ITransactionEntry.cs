using System;
using System.Collections.Generic;

namespace Aquirrel.Tracing.Internal
{
    public interface IRequestEntry
    {
        string Id { get; }
        string App { get; set; }
        DateTime BeginTime { get; set; }
        DateTime EndTime { get; set; }
        int Duration { get; }
        /// <summary>
        /// 当前请求的ip
        /// </summary>
        string ClientIp { get; set; }
        /// <summary>
        /// 当前机器ip
        /// </summary>
        string LocalIp { get; set; }
        /// <summary>
        /// 当前请求用户的真实ip
        /// </summary>
        string RealIp { get; set; }
        /// <summary>
        /// 请求序号
        /// </summary>
        string TraceDepth { get; set; }
        /// <summary>
        /// 请求跟踪id
        /// </summary>
        string TraceId { get; set; }
        /// <summary>
        /// 用户openid
        /// </summary>
        string UserOpenId { get; set; }
        /// <summary>
        /// 关联用户跟踪id
        /// </summary>
        string UserTraceId { get; set; }
        string AccessToken { get; set; }
        Dictionary<string, IRequestEntry> ChildRequest { get; }
        Dictionary<string, object> Datas { get; }
        Exception Exception { get; set; }

        IRequestEntry NewChildRequest();
    }
}