using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.MQ
{
    /// <summary>
    /// 订阅消息选项
    /// </summary>
    public class SubscribeOptions
    {
        /// <summary>
        /// 消息消费方式
        /// </summary>
        public MessageModel Model { get; set; } = MessageModel.Clustering;
        /// <summary>
        /// 并发消费数量
        /// </summary>
        public int BasicQos { get; set; } = 1;

        /// <summary>
        /// 消费失败的消息是否重新进入队列，false：不放入，true：放入。默认true
        /// </summary>
        public bool FailMesaageReQueue { get; set; } = true;
        /// <summary>
        /// 默认选项
        /// </summary>
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
