using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace ConfigurationManager.Core
{

    public class LazyConcurrentDictionary<TKey, TValue>
    {
        private readonly ConcurrentDictionary<TKey, Lazy<TValue>> concurrentDictionary;

        public LazyConcurrentDictionary()
        {
            this.concurrentDictionary = new ConcurrentDictionary<TKey, Lazy<TValue>>();
        }

        public bool TryGet(TKey key, out Lazy<TValue> value)
        {
            var lazyResult = this.concurrentDictionary.TryGetValue(key, out value);

            return lazyResult;
        } 

        public bool TryAdd(TKey key, Lazy<TValue> addValueFactory)
        {
            return this.concurrentDictionary.
                TryAdd(
                    key,
                    addValueFactory
                );
        }

        public TValue AddOrUpdate(TKey key, Lazy<TValue> addValueFactory, Func<TKey, Lazy<TValue>, Lazy<TValue>> updateValueFactory)
        {

            var lazyResult = this.concurrentDictionary.
                AddOrUpdate(
                    key,
                    addValueFactory,
                    updateValueFactory
                );

            return lazyResult.Value;
        }
    }
}
