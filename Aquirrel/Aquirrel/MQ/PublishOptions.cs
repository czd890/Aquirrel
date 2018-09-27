using System;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel.MQ
{
    /// <summary>
    /// 发布消息选项
    /// </summary>
    public class PublishOptions
    {
        /// <summary>
        /// 默认选项
        /// </summary>
        public static PublishOptions Default { get; } = new PublishOptions();
    }
}
