using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace Aquirrel.Tracing.Internal
{

    public class TracingSetting
    {
        IConfiguration _configuration;
        public IChangeToken ChangeToken { get; private set; }
        public TracingSetting()
        {

        }
        public TracingSetting(IConfiguration configuration)
        {
            _configuration = configuration;
            this.ChangeToken = _configuration.GetReloadToken();
            this.ChangeToken.RegisterChangeCallback(this.ResolveConf, null);
            this.ResolveConf(null);
        }
        void ResolveConf(object state)
        {
            this.SendTaskSleep = _configuration["SendTaskSleep"].ToInt(10000);
            this.ApiApp = _configuration["trace_api_App"];
            this.ApiName = _configuration["trace_api_Name"];
            this.MaxQueueWait = _configuration["MaxQueueWait"].ToInt(500);
            this.MaxBatchSubmitSize = _configuration["MaxBatchSubmitSize"].ToInt(10);
        }
        /// <summary>
        /// 毫秒
        /// </summary>
        public int SendTaskSleep { get; set; }
        /// <summary>
        /// 队列最多等待数量
        /// </summary>
        public int MaxQueueWait { get; set; }
        /// <summary>
        /// 最大批量提交数量
        /// </summary>
        public int MaxBatchSubmitSize { get; set; }

        public string ApiApp { get; set; }
        public string ApiName { get; set; }
    }
}
