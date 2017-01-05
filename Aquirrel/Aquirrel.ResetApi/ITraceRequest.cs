using System;

namespace Aquirrel.ResetApi
{
    public interface ITraceRequest
    {
        string currentId { get; }
        string parentId { get; }
    }
}