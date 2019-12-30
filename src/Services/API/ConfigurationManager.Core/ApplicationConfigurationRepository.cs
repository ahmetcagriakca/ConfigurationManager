using ConfigurationManager.Core.Data;
using ConfigurationManager.Core.Exceptions;
using ConfigurationManager.Core.Model;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigurationManager.Core
{
    public interface IApplicationConfigurationRepository
    {
        Task<List<ApplicationConfiguration>> GetAllAsync();
        Task<ApplicationConfiguration> GetByIdAsync(object id);
        Task<ApplicationConfiguration> AddAsync(ApplicationConfiguration entity);
        Task<ApplicationConfiguration> GetByNameAsync(string name, string applicationName);
        Task<ApplicationConfiguration> UpdateAsync(long id, ApplicationConfiguration entity);
        Task<List<ApplicationConfiguration>> GetByApplicationAsync(string applicationName);
        Task FindAndDeleteAsync(string key);
        Task DeleteAsync(int id);


    }
    public class ApplicationConfigurationRepository : MongoRepository<ApplicationConfiguration>, IApplicationConfigurationRepository
    {
        public ApplicationConfigurationRepository(
            IMongoDbContextLocator mongoDbContext,
            IMongoRepositoryEventHandler<ApplicationConfiguration> repositoryEventHandler
        ) : base(mongoDbContext, repositoryEventHandler)
        {

        }

        public virtual async Task<List<ApplicationConfiguration>> GetAllAsync()
        {
            return await GetCollection.AsQueryable().ToListAsync();
        }

        public virtual async Task<long> Count(string name, string applicationName)
        {
            var filter = Builders<ApplicationConfiguration>
                .Filter
                .And(
                    Builders<ApplicationConfiguration>
                        .Filter
                        .Eq("Name", name),
                    Builders<ApplicationConfiguration>
                        .Filter
                        .Eq("ApplicationName", applicationName)
                );
            return await GetCollection.CountDocumentsAsync(filter);
        }
        public override async Task<ApplicationConfiguration> AddAsync(ApplicationConfiguration entity)
        {
            if (await Count(entity.Name, entity.ApplicationName) > 0)
            {
                throw new ValueAlreadyExistsException("Value Already Exists");
            }
            else
            {
                repositoryEventHandler.OnInserting(entity);
                await GetCollection.InsertOneAsync(entity);
            }

            return entity;
        }

        public virtual async Task<ApplicationConfiguration> UpdateAsync(long id, ApplicationConfiguration entity)
        {

            var countFilter = Builders<ApplicationConfiguration>
                .Filter
                .And(
                    Builders<ApplicationConfiguration>
                        .Filter
                        .Where(en => en.Id != entity.Id),
                    Builders<ApplicationConfiguration>
                        .Filter
                        .Where(en => en.Name == entity.Name),
                    Builders<ApplicationConfiguration>
                        .Filter
                        .Where(en => en.ApplicationName == entity.ApplicationName)
                );
            var hasValue = await GetCollection.CountDocumentsAsync(countFilter);
            if (hasValue > 0)
            {
                throw new ValueAlreadyExistsException("Value Already Exists");
            }
            else
            {
                var filter = Builders<ApplicationConfiguration>.Filter.Where(en => en.Id == id);
                await GetCollection.FindOneAndReplaceAsync(filter, entity);
                return entity;
            }
        }

        public virtual async Task<ApplicationConfiguration> GetByNameAsync(string name, string applicationName)
        {
            var filter = Builders<ApplicationConfiguration>.Filter.And(Builders<ApplicationConfiguration>.Filter.Eq("Name", name)
                , Builders<ApplicationConfiguration>.Filter.Eq("ApplicationName", applicationName));
            var list = await GetCollection.Find(filter)
                .ToListAsync();
            return list.FirstOrDefault();
        }

        public virtual async Task<List<ApplicationConfiguration>> GetByApplicationAsync(string applicationName)
        {
            var filter = Builders<ApplicationConfiguration>.Filter.Eq("ApplicationName", applicationName);
            var list = await GetCollection.Find(filter)
                .ToListAsync();
            return list;
        }

        public virtual async Task FindAndDeleteAsync(string key)
        {
            var filter = Builders<ApplicationConfiguration>.Filter.Where(en => en.Name == key);
            await GetCollection.FindOneAndDeleteAsync(filter);
        }

        public virtual async Task DeleteAsync(int id)
        {
            var filter = Builders<ApplicationConfiguration>.Filter.Where(en => en.Id == id);
            await GetCollection.DeleteOneAsync(filter);
        }
    }
}
