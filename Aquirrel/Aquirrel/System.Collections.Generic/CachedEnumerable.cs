using System;
using System.Collections.Generic;
using System.Text;

namespace System.Collections.Generic
{
    public class CachedEnumerable<T> : IEnumerable<T>
    {
        IEnumerator<T> _enumerator;
        readonly List<T> _cache = new List<T>();

        public CachedEnumerable(IEnumerable<T> enumerable)
            : this(enumerable.GetEnumerator()) { }

        public CachedEnumerable(IEnumerator<T> enumerator) => _enumerator = enumerator;
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        public IEnumerator<T> GetEnumerator()
        {
            int index = 0;

            while (true)
            {
                if (index < _cache.Count)
                {
                    yield return _cache[index];
                    index = index + 1;
                }
                else
                {
                    if (_enumerator.MoveNext())
                    {
                        _cache.Add(_enumerator.Current);
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
        }
    }

    public class CachedMapEnumerable<TSource, TDesc> : IEnumerable<TDesc>
    {
        IEnumerator<TSource> _enumerator;
        readonly List<TDesc> _cache = new List<TDesc>();
        readonly Func<TSource, TDesc> converter;

        public CachedMapEnumerable(IEnumerable<TSource> enumerable, Func<TSource, TDesc> converter)
            : this(enumerable.GetEnumerator()) { this.converter = converter; }

        public CachedMapEnumerable(IEnumerator<TSource> enumerator) => _enumerator = enumerator;
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
        public IEnumerator<TDesc> GetEnumerator()
        {
            int index = 0;

            while (true)
            {
                if (index < _cache.Count)
                {
                    yield return _cache[index];
                    index = index + 1;
                }
                else
                {
                    if (_enumerator.MoveNext())
                    {
                        _cache.Add(this.converter(_enumerator.Current));
                    }
                    else
                    {
                        yield break;
                    }
                }
            }
        }
    }
}
