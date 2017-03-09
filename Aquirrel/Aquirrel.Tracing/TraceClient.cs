using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquirrel.Internal;
using Aquirrel.Tracing.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Framework.DependencyInjection;
namespace Aquirrel.Tracing
{
    public class TraceClient : ITraceClient
    {
        ILogger _logger;
        IReportClient _reportClient;
        public TraceClient(ILogger<TraceClient> logger, IReportClient reportClient)
        {
            this._logger = logger;
            this._reportClient = reportClient;
        }

        public TraceRoot Current { get { return TraceRoot.ALS.Value; } }
        public Task<TraceRoot> CreateTransaction(string app, string name)
        {
            return this.CreateTransaction(app, name, "", "");
        }

        //TODO 重写组装TraceEventEntry 等model

        public async Task<TraceRoot> CreateTransaction(string app, string name, string traceId, string parentId)
        {
            var als = new TraceRoot();
            als.App = app;
            als.Name = name;
            als.TraceId = traceId.IsNullOrEmpty() ? Guid.NewGuid().ToString() : traceId;
            als.ParentId = parentId;
            als.LastTime = DateTime.Now;
            als.LocalIp = await LocalIp.GetLocalIPV4();

            return als;
        }
        public void Begin(TraceRoot root)
        {
            TraceRoot.ALS.Value = root;
            if (this._reportClient == null)
                return;
            this._reportClient.Report(TraceRoot.ALS.Value);
        }
        public void Complete()
        {
            if (this._reportClient == null)
                return;
            this._reportClient.Report(TraceRoot.ALS.Value);
        }

        public void Event(string eventName)
        {
            if (this._reportClient == null)
                return;
            this._reportClient.Report(TraceRoot.ALS.Value, eventName);
        }

        public void Exception(Exception ex)
        {
            this.Exception(ex.Message, ex);
        }

        public void Exception(string message, Exception ex)
        {
            if (this._reportClient == null)
                return;
            this._reportClient.Report(TraceRoot.ALS.Value, message, ex);
        }


    }
}
