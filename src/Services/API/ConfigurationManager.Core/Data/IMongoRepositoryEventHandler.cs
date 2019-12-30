using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationManager.Core.Data
{
    /// <summary>
    /// Mongo Repository event managed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMongoRepositoryEventHandler<T>
    {
        void OnInserting(T collection);
    }
}
