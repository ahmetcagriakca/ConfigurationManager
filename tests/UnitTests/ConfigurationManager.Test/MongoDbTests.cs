using ConfigurationManager.Core;
using ConfigurationManager.Core.Data;
using ConfigurationManager.Core.Exceptions;
using ConfigurationManager.Core.Model;
using Xunit;

namespace ConfigurationManager.Test
{
    public class MongoDbTests
    {
        private static string mongoConnectionString = "mongodb://130.211.62.35:32768/";

        private IApplicationConfigurationRepository applicationConfigurationRepository;
        public MongoDbTests()
        {
            MongoDbContext mongoDbContext = new MongoDbContext(mongoConnectionString, "ConfigurationDb");
            IMongoDbContextLocator mongoDbContextLocator = new DbContextLocator(mongoDbContext);
            IMongoRepositorySequence<ApplicationConfiguration> mongoRepositorySequence = new MongoRepositorySequence<ApplicationConfiguration>(mongoDbContextLocator);
            IMongoRepositoryEventHandler<ApplicationConfiguration> mongoRepositoryEventHandler = new MongoRepositoryEvent<ApplicationConfiguration>(mongoRepositorySequence);
            applicationConfigurationRepository = new ApplicationConfigurationRepository(mongoDbContextLocator, mongoRepositoryEventHandler);
            var newConfiguration = new ApplicationConfiguration()
            {
                Name = "testKey",
                Value = "Value",
                ApplicationName = "Service-A",
                Type = "string"

            };
            try
            {
                var addResult = applicationConfigurationRepository.AddAsync(newConfiguration).Result;
            }
            catch (System.Exception)
            {
            }
        }

        [Fact]
        public void MongoConnectionTest()
        {
            MongoDbContext mongoDbContext = new MongoDbContext(mongoConnectionString, "ConfigurationDb");
            Assert.True(mongoDbContext.CheckConnection());
        }

        /// <summary>
        /// adding value to mongo repository after 
        /// </summary>
        [Fact]
        public void CheckExistingConfigurationAdd()
        {
            var newConfiguration = new ApplicationConfiguration()
            {
                Name = "testKey",
                Value = "Value",
                ApplicationName = "Service-A",
                Type = "string"

            };
            Assert.ThrowsAsync<ValueAlreadyExistsException>(async () =>
               {
                  await applicationConfigurationRepository.AddAsync(newConfiguration);
               });
        }
    }
}
