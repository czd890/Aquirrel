using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel.MQ.Internal
{
    public interface IMQ
    {
        void Subscribe(string productId, string topic, Func<BasicDeliverEventArgs, bool> action, SubscribeOptions options = null);

        void Publish(string productId, string topic, string tag, string id, byte[] body, IBasicProperties basicProperties, PublishOptions options = null);
    }
}
