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

        void Report(TraceRoot als, IEnumerable<KeyValuePair<string, string>> datas, bool hasRoot = false)
        {
            dynamic dy = new System.Dynamic.ExpandoObject();
            if (hasRoot)
            {
                dy.app = als.App;
                dy.name = als.Name;
                dy.userid = als.UserId;
                dy.localip = als.LocalIp;
                dy.clientip = als.ClientIp;
                dy.parentid = als.ParentId;
                dy.data = als.ExtendData;
            }
            else
            {
                dy.data = datas;
            }
            dy.seq = als.Seq;
            dy.traceid = als.TraceId;
            dy.time = DateTime.Now;
            if (this.workAdd.Count > this._conf.MaxQueueWait)
            {
                object result;
                this.workAdd.TryDequeue(out result);
            }
            this.workAdd.Enqueue(dy);

        }
        static List<object> _curCont = new List<object>();
        async void Work()
        {
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
                    _curCont.Clear();
                    var _i = 0;
                    while (!this.workRunning.IsEmpty && _i < this._conf.MaxBatchSubmitSize)
                    {
                        object obj;
                        if (this.workRunning.TryDequeue(out obj))
                        {
                            _curCont.Add(obj);
                            _i++;
                        }

                    }


                    int i = 2;
                    while (i > 0)
                    {
                        try
                        {
                            i--;

                            if (!await this.Report(_curCont))
                                Thread.Sleep(1000);
                            else
                                break;
                        }
                        catch (Exception ex)
                        {
                            this._logger.LogTrace("trace log report error. {0}.", ex.Message);
                            Thread.Sleep(this._conf.SendTaskSleep);
                        }
                    }
                }
            }
        }
        protected virtual async Task<bool> Report(List<dynamic> datas)
        {
            var req = new ReportRequest(HttpMethod.Post, this._conf.ApiApp, this._conf.ApiName);
            req.input = datas;
            var apiClient = _sp.GetService<IApiClient>();
            var r = await apiClient.ExecuteAsync(req);

            if (r.resCode == 200)
            {
                return true;
            }
            else
            {
                this._logger.LogTrace("trace log report work error. {0}.", r?.msg);
            }

            return false;
        }

        public void Report(TraceRoot root)
        {
            this.Report(root, null, true);
        }

        public void Report(TraceRoot root, string message)
        {
            this.Report(root, new List<KeyValuePair<string, string>>() { { "message", message } });
        }

        public void Report(TraceRoot root, string message, Exception exception)
        {
            this.Report(root, new List<KeyValuePair<string, string>>() { { "message", message }, { "exception", exception.ToString() } });
        }

        public void Report(TraceRoot root, Exception exception)
        {
            this.Report(root, exception.Message, exception);
        }
    }
}
