using ConfigurationManager.Core.Model;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigurationManager.Core.Data
{
    public interface IMongoRepository<T>
    {
        Task<T> GetByIdAsync(object id);
        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(long id, T entity);
    }

    public class MongoRepository<T> : IMongoRepository<T> where T : IEntity<long>
    {
        protected static IMongoCollection<T> Collection;
        private readonly MongoDbContext mongoDbContext;
        protected readonly IMongoRepositoryEventHandler<T> repositoryEventHandler;

        public virtual IMongoCollection<T> GetCollection => Collection;
        protected static string collectionName = typeof(T).Name.ToLower();

        public MongoRepository(
            IMongoDbContextLocator mongoDbContext,
            IMongoRepositoryEventHandler<T> repositoryEventHandler
        )
        {
            Collection = mongoDbContext.Current.Database.GetCollection<T>(collectionName);
            this.mongoDbContext = mongoDbContext.Current;
            this.repositoryEventHandler = repositoryEventHandler;
        }

        public virtual async Task<T> GetByIdAsync(object id)
        {
            var filter = Builders<T>.Filter.Eq("_id", id);
            var list = await GetCollection.Find(filter)
                .ToListAsync();
            return list.FirstOrDefault();
        }


        public virtual async Task<T> AddAsync(T entity)
        {
            repositoryEventHandler.OnInserting(entity);
            await GetCollection.InsertOneAsync(entity);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(long id, T entity)
        {
            var filter = Builders<T>.Filter.Where(en => en.Id == id);
            await GetCollection.FindOneAndReplaceAsync(filter, entity);
            return entity;
        }
    }
}
