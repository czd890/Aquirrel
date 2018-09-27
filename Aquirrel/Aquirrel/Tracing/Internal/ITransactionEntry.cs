using System;
using System.Collections.Generic;

namespace Aquirrel.Tracing.Internal
{
    /// <summary>
    /// RPC链路跟踪对象
    /// </summary>
    public interface IRequestEntry
    {
        /// <summary>
        /// 当前操作唯一id
        /// </summary>
        string Id { get; }
        /// <summary>
        /// 当前app名称（域名地址）
        /// </summary>
        string App { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        DateTime BeginTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
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
        /// <summary>
        /// 授权token
        /// </summary>
        string AccessToken { get; set; }
        /// <summary>
        /// 已执行的子请求
        /// </summary>
        Dictionary<string, IRequestEntry> ChildRequest { get; }
        /// <summary>
        /// 扩展数据
        /// </summary>
        Dictionary<string, object> Datas { get; }
        /// <summary>
        /// 请求异常对象
        /// </summary>
        Exception Exception { get; set; }
        /// <summary>
        /// 创建一个新的子请求对象
        /// </summary>
        /// <returns></returns>
        IRequestEntry NewChildRequest();
    }
}