using ConfigurationManager.Api.Model;
using ConfigurationManager.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigurationManager.Api.Utility
{
    public static class ConfigurationExtensions
    {

        public static ConfigurationEnvironment GetConfigurationConfig(this IConfiguration configuration, string name)
        {
            var section = configuration.GetSection(name);
            var config = new ConfigurationEnvironment();
            section.Bind(config);
            if (configuration["DB_APPLICATION_NAME"] != null)
            {
                config.ApplicationName = configuration["DB_APPLICATION_NAME"];
            }
            if (configuration["DB_CONNECTION_STRING"] != null)
            {
                config.ConnectionString = configuration["DB_CONNECTION_STRING"];
            }
            if (configuration["DB_DATABASE_NAME"] != null)
            {
                config.DatabaseName = configuration["DB_DATABASE_NAME"];
            }
            if (configuration["DB_REFRESH_TIME_INTERVAL"] != null)
            {
                config.RefreshTimeInterval = Convert.ToInt32(configuration["DB_REFRESH_TIME_INTERVAL"]);
            }
            return config;
        }

        public static IServiceCollection AddConfigurationManager(this IServiceCollection services, ConfigurationEnvironment config)
        {
            if (config.IsValid(out string message))
            {
                services.AddSingleton(config);
            }

            services.AddSingleton<IConfigurationReader, ConfigurationReader>(provider =>
            {
                return new ConfigurationReader(config.ApplicationName, config.ConnectionString, config.RefreshTimeInterval);
            });
            return services;
        }
    }
}
