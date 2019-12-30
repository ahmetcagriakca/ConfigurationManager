using ConfigurationManager.Core.Data;
using ConfigurationManager.Core.Model;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConfigurationManager.Core
{
    public class ConfigurationReader : IConfigurationReader
    {

        #region Properties
        private LazyConcurrentDictionary<string, object> concurrentDictionary = new LazyConcurrentDictionary<string, object>();
        private readonly string applicationName;
        private readonly string connectionString;
        private readonly int refreshTimerIntervalInMs;
        private IApplicationConfigurationRepository applicationConfigurationRepository;
        private ITypeResolver typeResolver;
        private IMongoDbContextLocator mongoDbContextLocator;
        #endregion

        #region Ctor
        public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
        {
            this.applicationName = applicationName;
            this.connectionString = connectionString;
            this.refreshTimerIntervalInMs = refreshTimerIntervalInMs;

            /// type resolver created
            typeResolver = new TypeResolver();
            #region Mongo Db Initialization
            MongoDbContext mongoDbContext = new MongoDbContext(connectionString, "ConfigurationDb");
            if (!mongoDbContext.CheckConnection())
            {
                throw new MongoConnectionException("Mongo connection failed");
            }
            mongoDbContextLocator = new DbContextLocator(mongoDbContext);
            IMongoRepositorySequence<ApplicationConfiguration> mongoRepositorySequence = new MongoRepositorySequence<ApplicationConfiguration>(mongoDbContextLocator);
            IMongoRepositoryEventHandler<ApplicationConfiguration> mongoRepositoryEventHandler = new MongoRepositoryEvent<ApplicationConfiguration>(mongoRepositorySequence);
            applicationConfigurationRepository = new ApplicationConfigurationRepository(mongoDbContextLocator, mongoRepositoryEventHandler);
            #endregion

            #region Initializing and resfrehing operations
            Initialize();

            IRefreshTimeManager refreshTimeManager = new RefreshTimeManager(refreshTimerIntervalInMs);
            refreshTimeManager.StartTimer(async () =>
            {
                await UpdateStoreValues(this.applicationName);
            });
            #endregion
        }
        #endregion
        private void Initialize()
        {
            var task = Task.Run(async () => { await InitializeConfigurations(this.applicationName); });
            Task.WaitAll(task);
        }

        /// <summary>
        /// Initializing values for application
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        private async Task InitializeConfigurations(string applicationName)
        {
            var configurations = await applicationConfigurationRepository.GetByApplicationAsync(applicationName);
            configurations.ForEach(en =>
                {
                    concurrentDictionary.TryAdd(en.Name, new Lazy<object>(() =>
                    {

                        return typeResolver.ValueConvertToType(en.Value, en.Type);
                    }, LazyThreadSafetyMode.ExecutionAndPublication));
                }
            );
        }

        /// <summary>
        /// Updating and 
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        private async Task UpdateStoreValues(string applicationName)
        {
            if (mongoDbContextLocator.Current.CheckConnection())
            {
                var configurations = await applicationConfigurationRepository.GetByApplicationAsync(applicationName);
                configurations.ForEach(en =>
                {
                    concurrentDictionary.AddOrUpdate(
                        en.Name,
                        new Lazy<object>(() => { return typeResolver.ValueConvertToType(en.Value, en.Type); },
                            LazyThreadSafetyMode.ExecutionAndPublication),
                        (k, v) => new Lazy<object>(() => { return typeResolver.ValueConvertToType(en.Value, en.Type); },
                            LazyThreadSafetyMode.ExecutionAndPublication)
                    );
                });
            }
        }

        public T GetValue<T>(string key)
        {
            if (concurrentDictionary.TryGet(key, out Lazy<object> lazyValue))
            {
                return (T)lazyValue.Value;
            }
            else
            {
                throw new ValueNotFoundedException($"{key} not founded for application:{applicationName}");
            }
        }

    }
}
