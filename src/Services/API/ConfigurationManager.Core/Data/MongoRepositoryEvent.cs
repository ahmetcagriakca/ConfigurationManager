using ConfigurationManager.Core.Model;
using System;

namespace ConfigurationManager.Core.Data
{
    public class MongoRepositoryEvent<T> : IMongoRepositoryEventHandler<T>
    {
        private readonly IMongoRepositorySequence<T> repositorySequence;

        public MongoRepositoryEvent(
            IMongoRepositorySequence<T> repositorySequence)
        {
            this.repositorySequence = repositorySequence ?? throw new ArgumentNullException(nameof(repositorySequence));
        }

        public void OnInserting(T entity)
        {
            if (typeof(IEntity).IsAssignableFrom(entity.GetType()))
            {
                 var _entity = entity as IEntity<long>;
                _entity.IsActive = true;
                if (typeof(IEntity<long>).IsAssignableFrom(entity.GetType()))
                {
                    var myEntity = entity as IEntity<long>;
                    myEntity.Id = repositorySequence.GetNextSequence().ToLong();
                }
            }
        }
    }
}
