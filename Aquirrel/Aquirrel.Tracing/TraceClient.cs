using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aquirrel.Tracing.Internal;
using Microsoft.Extensions.Logging;
using Aquirrel.Tools;

namespace Aquirrel.Tracing
{
    public class TraceClient : ITraceClient
    {
        ILogger _logger;
        //IReportClient _reportClient;
        ReportClient _reportClient;
        public TraceClient(ILogger<TraceClient> logger, IReportClient reportClient)
        {
            this._logger = logger;
            this._reportClient = (ReportClient)reportClient;
        }

        public TransactionEntry Current { get { return TransactionEntry.ALS.Value; } }
        public TransactionEntry CreateTransaction(string app, string name)
        {
            return this.CreateTransaction(app, name, "", 0);
        }

        //TODO 重写组装TraceEventEntry 等model

        public TransactionEntry CreateTransaction(string app, string name, string traceId, int tracelevel, string clientIp = "")
        {
            var als = TransactionEntry.ALS.Value = new TransactionEntry();
            als.App = app;
            als.Name = name;
            als.TraceId = traceId.IsNullOrEmpty() ? Guid.NewGuid().ToString() : traceId;
            als.TraceLevel = tracelevel;
            als.LastTime = DateTime.Now;
            als.LocalIp = LocalIp.GetLocalIPV4().ConfigureAwait(false).GetAwaiter().GetResult();
            als.ClientIp = clientIp;
            als.ExtendData.isFirst = false;
            als.ExtendData.seq = 0;
            this._reportClient.Report(new TraceEventEntry() { ALS = TransactionEntry.ALS.Value, Event = "BEGIN" });
            return als;
        }
        public void Complete()
        {
            this._reportClient.Report(new TraceEventEntry() {  ALS = TransactionEntry.ALS.Value, Event = "END" });
        }

        public void Event(string eventName)
        {
            this._reportClient.Report(new TraceEventEntry() { ALS = TransactionEntry.ALS.Value, Event = eventName });
        }

        public void Exception(Exception ex)
        {
            this.Exception("", ex);
        }
        public void Exception(string message)
        {
            this.Exception(message, null);
        }

        public void Exception(string message, Exception ex)
        {
            this._reportClient.Report(new TraceExceptionEntry() { ALS = TransactionEntry.ALS.Value, Message = message, EX = ex });
        }





    }
}
