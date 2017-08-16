using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Aquirrel.EntityFramework.Internal
{
    public class InternalScopeServiceContainer : ConcurrentDictionary<string, object>, IDisposable
    {
        public void Dispose()
        {
            try
            {
                foreach (var item in this)
                {
                    try
                    {
                        (item.Value as IDisposable)?.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            catch
            {
            }
        }
    }
}
