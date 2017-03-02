﻿using System;

namespace Aquirrel.MQ
{
    public interface IEventBus
    {
        void Exit();
        void Publish<T>(string productId, string topic, string tag, string id, T message);
        void Subscribe<T>(string productId, string topic, Func<T, bool> action, SubscribeOptions options = null);
    }
}