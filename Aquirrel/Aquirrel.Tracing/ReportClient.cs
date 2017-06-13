using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Aquirrel.ResetApi;
using Aquirrel.ResetApi.Internal;
using Aquirrel.Tracing.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aquirrel.Tracing
{
    /// <summary>
    /// 上报服务器
    /// </summary>
    public class ReportClient : IReportClient
    {
        ILogger _logger;
        IServiceProvider _sp;
        //IRestApiResolveApiUrl _resovleReportApi;
        TracingSetting _conf;
        Thread workThread;
        System.Collections.Concurrent.ConcurrentQueue<object> workAdd = new System.Collections.Concurrent.ConcurrentQueue<object>();
        System.Collections.Concurrent.ConcurrentQueue<object> workRunning = new System.Collections.Concurrent.ConcurrentQueue<object>();
        public ReportClient(
            ILogger<ReportClient> logger,
            //IRestApiResolveApiUrl resovleReportApi,
            IServiceProvider sp,
            TracingSetting setting)
        {
            this._logger = logger;
            //this._resovleReportApi = resovleReportApi;
            this._sp = sp;
            this._conf = setting;

            workThread = new Thread(this.Work);
            workThread.Start();
        }

        //TODO 消息的格式化独立出来可替换
        public void Report(TraceCompleteEntry entry)
        {
            this._logger.LogTrace(nameof(entry) + " " + entry.ToJson());
            Report(entry.ALS, new List<KeyValuePair<string, string>>() { { "msg", entry.Message } });
        }

        public void Report(TraceExceptionEntry entry)
        {
            this._logger.LogTrace(nameof(entry) + " " + entry.ToJson());
            Report(entry.ALS, new List<KeyValuePair<string, string>>() { { "msg", entry.Message }, { "ex", entry.EX?.ToString() } });
        }

        public void Report(TraceEventEntry entry)
        {
            this._logger.LogTrace(nameof(entry) + " " + entry.ToJson());
            Report(entry.ALS, new List<KeyValuePair<string, string>>() { { "event", entry.Event } });
        }

        void Report(TransactionEntry als, IEnumerable<KeyValuePair<string, string>> datas)
        {
            if (als == null)
            {
                this._logger.LogDebug("trace event als is null.");
                return;
            }
            als.ExtendData.seq++;
            var now = als.LastTime = DateTime.Now;

            dynamic dy = new System.Dynamic.ExpandoObject();
            if (!als.ExtendData.isFirst)
            {
                dy.app = als.App;
                dy.name = als.Name;
                dy.clientip = als.ClientIp;
                dy.localip = als.LocalIp;
                dy.level = als.TraceLevel;
            }
            dy.seq = als.ExtendData.seq;
            dy.traceid = als.TraceId;
            dy.data = datas;
            dy.time = now;
            if (this.workAdd.Count > this._conf.MaxQueueWait)
            {
                object result;
                this.workAdd.TryDequeue(out result);
            }
            this.workAdd.Enqueue(dy);
        }

        void Work()
        {
            //TODO 事件批量压缩上报
            var apiClient = _sp.GetService<IApiClient>();
            var req = new ReportRequest(HttpMethod.Post, this._conf.ApiApp, this._conf.ApiName);
            while (true)
            {
                if (this.workAdd.IsEmpty)
                {
                    Thread.Sleep(this._conf.SendTaskSleep);
                    continue;
                }

                var work = this.workRunning;
                this.workRunning = this.workAdd;
                this.workAdd = work;

                while (!this.workRunning.IsEmpty)
                {
                    object obj;
                    if (this.workRunning.TryDequeue(out obj))
                    {
                        req.input = obj;
                        int i = 2;
                        bool iss = false;
                        string error = "";
                        while (i > 0)
                        {
                            try
                            {
                                i--;
                                var r = /*await apiClient.ExecuteAsync(req);*/
                                    new ResponseBase();
                                if (r.resCode == 200)
                                {
                                    iss = true;
                                    break;
                                }
                                error = r.msg;
                                Thread.Sleep(1000);
                            }
                            catch (Exception ex)
                            {
                                error = ex.Message;
                                Thread.Sleep(this._conf.SendTaskSleep);
                            }
                        }
                        if (iss)
                        {
                            this._logger.LogTrace("trace log report error. {0}.", error);
                        }
                    }
                }
            }
        }
    }
}
