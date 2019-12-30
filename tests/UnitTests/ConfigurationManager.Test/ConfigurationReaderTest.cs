using ConfigurationManager.Core;
using ConfigurationManager.Core.Data;
using ConfigurationManager.Core.Model;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace ConfigurationManager.Test
{
    public class ConfigurationReaderTests
    {
        ITestOutputHelper output;
        private static string mongoConnectionString = "mongodb://130.211.62.35:32768/";
        private IApplicationConfigurationRepository applicationConfigurationRepository;
        public ConfigurationReaderTests(ITestOutputHelper output)
        {
            this.output = output;
            MongoDbContext mongoDbContext = new MongoDbContext(mongoConnectionString, "ConfigurationDb");
            IMongoDbContextLocator mongoDbContextLocator = new DbContextLocator(mongoDbContext);
            IMongoRepositorySequence<ApplicationConfiguration> mongoRepositorySequence = new MongoRepositorySequence<ApplicationConfiguration>(mongoDbContextLocator);
            IMongoRepositoryEventHandler<ApplicationConfiguration> mongoRepositoryEventHandler = new MongoRepositoryEvent<ApplicationConfiguration>(mongoRepositorySequence);
            applicationConfigurationRepository = new ApplicationConfigurationRepository(mongoDbContextLocator, mongoRepositoryEventHandler);
            InitializeTestDb();
        }

        private void InitializeTestDb()
        {
            var task = Task.Run(async () =>
            {

                var newConfiguration = new ApplicationConfiguration()
                {
                    Name = "SiteName",
                    Value = "cm",
                    ApplicationName = "SERVICE-A",
                    Type = "String"
                };
                try
                {
                    await applicationConfigurationRepository.AddAsync(newConfiguration);
                }
                catch (System.Exception)
                {
                }
                newConfiguration = new ApplicationConfiguration()
                {
                    Name = "IsBasketEnabled",
                    Value = "1",
                    ApplicationName = "SERVICE-B",
                    Type = "Boolean"
                };
                try
                {
                    await applicationConfigurationRepository.AddAsync(newConfiguration);
                }
                catch (System.Exception)
                {
                }
                newConfiguration = new ApplicationConfiguration()
                {
                    Name = "MaxItemCount",
                    Value = "50",
                    ApplicationName = "SERVICE-A",
                    Type = "Int"
                };
                try
                {
                    await applicationConfigurationRepository.AddAsync(newConfiguration);
                }
                catch (System.Exception)
                {
                }
                newConfiguration = new ApplicationConfiguration()
                {
                    Name = "HasConfiguration",
                    Value = "1",
                    ApplicationName = "SERVICE-A",
                    Type = "Boolean"
                };
                try
                {
                    await applicationConfigurationRepository.AddAsync(newConfiguration);
                }
                catch (System.Exception)
                {
                }
                newConfiguration = new ApplicationConfiguration()
                {
                    Name = "MaxShoppingAmount",
                    Value = "250.5",
                    ApplicationName = "SERVICE-A",
                    Type = "Double"
                };
                try
                {
                    await applicationConfigurationRepository.AddAsync(newConfiguration);
                }
                catch (System.Exception)
                {
                }
            });
            Task.WaitAll(task);
        }

        [Theory]
        [InlineData("SERVICE-C", "ConcurrentThreadCount", 5, "Int")]
        [InlineData("SERVICE-D", "RandomValue", 10, "Int")]
        public void AddValueToStorage(string applicationName, string key, int value, string type)
        {
            var task = Task.Run(async () =>
            {
                await applicationConfigurationRepository.FindAndDeleteAsync(key);
                var newConfiguration = new ApplicationConfiguration()
                {
                    Name = key,
                    Value = value.ToString(),
                    ApplicationName = applicationName,
                    Type = type
                };
                await applicationConfigurationRepository.AddAsync(newConfiguration);
            });
            Task.WaitAll(task);

            IConfigurationReader configurationReader = new ConfigurationReader(applicationName, mongoConnectionString, 1000);
            var result = configurationReader.GetValue<int>(key);
            Assert.True(result == value);
        }

        [Theory]
        [InlineData("SERVICE-A", "MaxItemCount", 5, "Int")]
        public void UpdateValueToStorageAndGetAfter(string applicationName, string key, int updateValue, string updatedType)
        {
            var task = Task.Run(async () =>
            {
                var configuration = await applicationConfigurationRepository.GetByNameAsync(key, applicationName);
                configuration.Value = updateValue.ToString();
                configuration.Type = updatedType;
                await applicationConfigurationRepository.UpdateAsync(configuration.Id, configuration);
            });
            Task.WaitAll(task);
            IConfigurationReader configurationReader = new ConfigurationReader(applicationName, mongoConnectionString, 1000);
            var result = configurationReader.GetValue<int>(key);
            Assert.True(result == updateValue);
        }

        [Theory]
        [InlineData("SERVICE-A", "SiteName", "cm", typeof(String))]
        [InlineData("SERVICE-A", "MaxItemCount", 5, typeof(Int32))]
        [InlineData("SERVICE-A", "HasConfiguration", true, typeof(Boolean))]
        [InlineData("SERVICE-A", "MaxShoppingAmount", 250.5, typeof(Double))]
        public void CheckInitilizedStoreValues(string applicationName, string key, object value, Type type)
        {
            IConfigurationReader configurationReader = new ConfigurationReader(applicationName, mongoConnectionString, 1000);
            switch (type.Name)
            {
                case "String":
                    {
                        var result = configurationReader.GetValue<string>(key);
                        Assert.True(result == value.ToString());
                    }

                    break;
                case "Int32":
                case "Int":
                    {
                        var result = configurationReader.GetValue<int>(key);
                        Assert.True(result == Convert.ToInt32(value));
                    }
                    break;
                case "Boolean":
                    {
                        var result = configurationReader.GetValue<bool>(key);
                        Assert.True(result == Convert.ToBoolean(value));
                    }
                    break;
                case "Double":
                    {
                        var result = configurationReader.GetValue<double>(key);
                        Assert.True(result == Convert.ToDouble(value));
                    }
                    break;
                default:
                    break;
            }
        }

        [Theory]
        [InlineData("SERVICE-A", "IsBasketEnabled")]
        [InlineData("SERVICE-B", "MaxItemCount")]
        public void CheckValueNotFounded(string applicationName, string key)
        {
            IConfigurationReader configurationReader = new ConfigurationReader(applicationName, mongoConnectionString, 1000);
            Assert.Throws<ValueNotFoundedException>(() => { configurationReader.GetValue<string>(key); });
        }

        [Theory]
        [InlineData("SERVICE-F", "OldValue", 5, "Int", 15, "Int")]
        public void CheckTimeIntervalWithValueChanging(string applicationName, string key, int value, string type, int newValue, string newType)
        {
            var task = Task.Run(async () =>
            {
                await applicationConfigurationRepository.FindAndDeleteAsync(key);
                var newConfiguration = new ApplicationConfiguration()
                {
                    Name = key,
                    Value = value.ToString(),
                    ApplicationName = applicationName,
                    Type = type
                };
                await applicationConfigurationRepository.AddAsync(newConfiguration);
            });
            Task.WaitAll(task);

            IConfigurationReader configurationReader = new ConfigurationReader(applicationName, mongoConnectionString, 5000);


            var result = configurationReader.GetValue<int>(key);

            Assert.True(result == value);

            var newTask = Task.Run(async () =>
            {
                var configuration = await applicationConfigurationRepository.GetByNameAsync(key, applicationName);
                configuration.Value = newValue.ToString();
                configuration.Type = newType;
                await applicationConfigurationRepository.UpdateAsync(configuration.Id, configuration);
            });
            Task.WaitAll(newTask);
            result = configurationReader.GetValue<int>(key);
            Assert.True(result == value);

            Thread.Sleep(5500);
            result = configurationReader.GetValue<int>(key);
            Assert.True(result == newValue);
        }
    }
}