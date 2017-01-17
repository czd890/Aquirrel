﻿using System;
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

        public TransactionEntry Current { get { return TransactionEntry.ALS.Value; } }
        public TransactionEntry CreateTransaction(string app, string name)
        {
            return this.CreateTransaction(app, name, "", 0);
        }

        //TODO 重写组装TraceEventEntry 等model

        public TransactionEntry CreateTransaction(string app, string name, string traceId, string parentId)
        {
            var als = TransactionEntry.ALS.Value = new TransactionEntry();
            als.App = app;
            als.Name = name;
            als.TraceId = traceId.IsNullOrEmpty() ? Guid.NewGuid().ToString() : traceId;
            als.ParentId = parentId;
            als.LastTime = DateTime.Now;
            als.LocalIp = LocalIp.GetLocalIPV4().ConfigureAwait(false).GetAwaiter().GetResult();
            als.ExtendData.isFirst = false;
            als.ExtendData.seq = 0;
            this._reportClient.Report(new TraceEventEntry() { Event = "BEGIN" });
            return als;
        }
        public void Complete()
        {
            this._reportClient.Report(new TraceEventEntry() { Event = "END" });
        }

        public void Event(string eventName)
        {
            this._reportClient.Report(new TraceEventEntry() { Event = eventName });
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
