using ConfigurationManager.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationManager.Core.Data;

namespace ConfigurationManager.Api.Model
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddMongoDbContext(this IServiceCollection services, ConfigurationEnvironment config)
        {
            if (config.IsValid(out string message))
            {
                services.AddSingleton(config);
            }

            services.AddSingleton<MongoDbContext>(provider =>
            {
                return new MongoDbContext(config.ConnectionString, config.DatabaseName);
            });
            return services;
        }
    }
}
