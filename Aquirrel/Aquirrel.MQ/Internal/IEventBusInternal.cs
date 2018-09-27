using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel.MQ.Internal
{
    /// <summary>
    /// 消息总线，发布、订阅。提供原始方法
    /// </summary>
    public interface IEventBusInternal
    {
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="productId">初始化配置内容中的productId</param>
        /// <param name="topic">消息名称</param>
        /// <param name="action">消息处理函数，返回true表示消息处理完毕，返回false或抛出异常表示消息处理失败，将根据消息的设置进行重试或者进入重试队列</param>
        /// <param name="options"></param>
        void Subscribe(string productId, string topic, Func<BasicDeliverEventArgs, bool> action, SubscribeOptions options = null);

        /// <summary>
        /// 发布消息
        /// </summary>
        /// <param name="productId">初始化配置内容中的productId</param>
        /// <param name="topic">主题</param>
        /// <param name="tag">路由</param>
        /// <param name="id">消息id，尽可能唯一，且和业务关联。方便对消息进行查询和轨迹跟踪</param>
        /// <param name="body">消息内容</param>
        /// <param name="basicProperties">消息属性</param>
        /// <param name="options"></param>
        void Publish(string productId, string topic, string tag, string id, byte[] body, IBasicProperties basicProperties, PublishOptions options = null);
    }
}
