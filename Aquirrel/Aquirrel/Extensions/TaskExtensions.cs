using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquirrel
{
    public static class TaskExtensions
    {
        private static readonly Task _defaultCompleted = Task.FromResult<AsyncVoid>(default(AsyncVoid));
        private struct AsyncVoid
        {
        }
        public static Task FromResult()
        {
            return _defaultCompleted;
        }
    }
}
