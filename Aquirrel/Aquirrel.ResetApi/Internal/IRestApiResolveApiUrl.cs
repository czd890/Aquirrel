using System;

namespace Aquirrel.ResetApi.Internal
{
    /// <summary>
    /// 使用app，apiname 解析出来真正的服务器地址。
    /// 可以做服务自动发现
    /// </summary>
    public interface IRestApiResolveApiUrl
    {
        Uri Resolve(string app, string apiName);
    }
}