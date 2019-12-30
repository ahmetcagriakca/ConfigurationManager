using System.Collections.Generic;
using ConfigurationManager.Api.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace ConfigurationManager.Api.Utility
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services, IdentityConfig config)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Version = "v1.0",
                    Title = "Configuration Manager Api",
                    Description = "Configuration Manager api",
                    TermsOfService = "None",
                });
                options.AddSecurityDefinition("oauth2", new OAuth2Scheme()
                {
                    Flow = "password",
                    AuthorizationUrl = $"{config.Authority}/connect/authorize",
                    Scopes = new Dictionary<string, string> {
                        { $"{config.Audience}", $"{config.Audience}"}
                    },
                    TokenUrl = $"{config.Authority}/connect/token"
                });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "oauth2", new string[] { } }
                });
            });
            return services;
        }


    }
}
