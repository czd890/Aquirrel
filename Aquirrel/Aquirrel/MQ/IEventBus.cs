using System;

namespace Aquirrel.MQ
{
    /// <summary>
    /// 消息总线，发布、订阅
    /// </summary>
    public interface IEventBus
    {
        void Exit();
        /// <summary>
        /// 发布消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productId">初始化配置内容中的productId</param>
        /// <param name="topic">主题</param>
        /// <param name="tag">路由</param>
        /// <param name="id">消息id，尽可能唯一，且和业务关联。方便对消息进行查询和轨迹跟踪</param>
        /// <param name="message">消息内容</param>
        /// <param name="options"></param>
        void Publish<T>(string productId, string topic, string tag, string id, T message, PublishOptions options = null);
        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="productId">初始化配置内容中的productId</param>
        /// <param name="topic">消息名称</param>
        /// <param name="action">消息处理函数，返回true表示消息处理完毕，返回false或抛出异常表示消息处理失败，将根据消息的设置进行重试或者进入重试队列</param>
        /// <param name="options"></param>
        void Subscribe<T>(string productId, string topic, Func<T, bool> action, SubscribeOptions options = null);
    }
}