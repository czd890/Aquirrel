using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aquirrel.Logger.File.Internal
{
    public class FileLogScope
    {
        private readonly string _name;
        private readonly object _state;

        internal FileLogScope(string name, object state)
        {
            _name = name;
            _state = state;
        }

        public FileLogScope Parent { get; private set; }
        private static AsyncLocal<FileLogScope> _value = new AsyncLocal<FileLogScope>();
        public static FileLogScope Current
        {
            set
            {
                _value.Value = value;
            }
            get
            {
                return _value.Value;
            }
        }

        public static IDisposable Push(string name, object state)
        {
            var temp = Current;
            Current = new FileLogScope(name, state);
            Current.Parent = temp;

            return new DisposableScope();
        }

        public override string ToString()
        {
            return _state?.ToString();
        }

        private class DisposableScope : IDisposable
        {
            public void Dispose()
            {
                Current = Current.Parent;
            }
        }
    }

}
