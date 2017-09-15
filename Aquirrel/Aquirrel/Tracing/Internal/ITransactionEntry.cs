using System;
using System.Collections.Generic;

namespace Aquirrel.Tracing.Internal
{
    public interface IRequestEntry
    {
        string App { get; set; }
        DateTime BeginTime { get; set; }
        DateTime EndTime { get; set; }
        TimeSpan Duration { get; }
        string ClientIp { get; set; }
        string LocalIp { get; set; }
        string TraceDepth { get; set; }
        string TraceId { get; set; }

        Dictionary<string, IRequestEntry> ChildRequest { get; }
        Dictionary<string, object> Datas { get; }
        Exception Exception { get; set; }

        IRequestEntry NewChildRequest();
    }
}