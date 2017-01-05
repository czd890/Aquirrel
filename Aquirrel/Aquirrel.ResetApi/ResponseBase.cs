using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Aquirrel.ResetApi
{

    public class ResponseBase : IResponseBase
    {
        public int error { get; set; }

        public string msg { get; set; }
    }
    public class ResponseBase<TData> : ResponseBase, IResponseBase<TData>
    {
        public TData data { get; set; }


    }
}
