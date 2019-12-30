using ConfigurationManager.Core.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConfigurationManager.Core.Data
{
    /// <summary>
    /// mongo collection identity column sequence value generator
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IMongoRepositorySequence<T>
    {
        long GetNextSequence();
    }
    public class MongoRepositorySequence<T> : IMongoRepositorySequence<T>
    {
        protected static IMongoCollection<MongoSequence> _collection;
        protected static string ReferenceCollectionName = typeof(T).Name.ToLower();
        public MongoRepositorySequence(IMongoDbContextLocator contextLocator)
        {
            _collection = contextLocator.Current.Database.GetCollection<MongoSequence>("sequences");
            var sequence = _collection.Find(x => x.Id == ReferenceCollectionName).ToList().FirstOrDefault();
            if (sequence == null)
            {
                _collection.InsertOne(new MongoSequence() { Id = ReferenceCollectionName, Sequence = 0 });
            }
        }

        public virtual long GetNextSequence()
        {
            string ReferenceCollectionName = typeof(T).Name.ToLower();
            var update = Builders<MongoSequence>.Update
                .Inc("Sequence", 1);
            var filter = Builders<MongoSequence>.Filter.Eq("_id", ReferenceCollectionName);
            var updateValue = _collection.UpdateOne(filter, update);
            return GetSequence(ReferenceCollectionName).ToLong();
        }

        private object GetSequence(string name)
        {
            var sequence = _collection.Find(x => x.Id == name).ToList().FirstOrDefault();
            return sequence.Sequence;
        }
    }

}
