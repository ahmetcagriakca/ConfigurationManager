using ConfigurationManager.Api.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigurationManager.Api.Utility
{
    public static class IdentityExtensions
    {


        public static IdentityConfig GetIdentityConfig(this IConfiguration configuration, string name)
        {
            var section = configuration.GetSection(name);
            var config = new IdentityConfig();
            section.Bind(config);
            if (configuration["IDENTITY_AUTHORITY"] != null)
            {
                config.Authority = configuration["IDENTITY_AUTHORITY"];
            }
            if (configuration["IDENTITY_AUDINCE"] != null)
            {
                config.Audience = configuration["IDENTITY_AUDINCE"];
            }

            return config;
        }


        public static IServiceCollection AddIdentityServer(this IServiceCollection services, IdentityConfig config)
        {
            services.AddSingleton(config);
            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options =>
                {
                    options.Authority = config.Authority;
                    options.RequireHttpsMetadata = false;
                    options.Audience = config.Audience;
                });
            return services;
        }


    }
}
