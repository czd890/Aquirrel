using System;

namespace Aquirrel.ResetApi
{
    internal interface ITraceRequest
    {
        Guid currentId { get; }
        Guid parentId { get; }
    }
}