using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.ResetApi.Internal
{
    /// <summary>
    /// 使用app，apiname 解析出来真正的服务器地址。
    /// 可以做服务自动发现
    /// </summary>
    public class RestApiResolveApiUrl : IRestApiResolveApiUrl
    {
        public Uri Resolve(string app, string apiName)
        {
            UriBuilder ub = new UriBuilder(app);
            //ub.Host = app;
            ub.Path = apiName;
            return ub.Uri;
        }
    }
}
