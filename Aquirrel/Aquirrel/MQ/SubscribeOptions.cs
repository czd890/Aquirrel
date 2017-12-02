using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.MQ
{
    public class SubscribeOptions
    {
        public MessageModel Model { get; set; } = MessageModel.Clustering;
        public bool ShardingConn { get; set; } = true;

        public int BasicQos { get; set; } = 1;

        public bool FailMesaageReQueue { get; set; } = true;

        public static SubscribeOptions Default { get; } = new SubscribeOptions() { };
    }
    /// <summary>
    /// 消息消费方式
    /// </summary>
    public enum MessageModel
    {
        /// <summary>
        /// 广播消费
        /// </summary>
        Broadcasting = 1,
        /// <summary>
        /// 集群消费,默认方式
        /// </summary>
        Clustering = 2
    }
}
