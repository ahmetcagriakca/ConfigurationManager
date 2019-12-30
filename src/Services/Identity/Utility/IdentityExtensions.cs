using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigurationManager.Api.Model;
using IdentityServer.UserServices;
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
            if (configuration["IDENTITY_CERTIFICATE_PASSWORD"] != null)
            {
                config.CertificatePassword = configuration["IDENTITY_CERTIFICATE_PASSWORD"];
            }
            if (configuration["IDENTITY_CERTIFICATE_PATH"] != null)
            {
                config.CertificatePath = configuration["IDENTITY_CERTIFICATE_PATH"];
            }
            return config;
        }

        public static IIdentityServerBuilder AddCustomUserStore(this IIdentityServerBuilder builder)
        {
            return
                builder
                    .AddProfileService<CustomProfileService>()
                    .AddResourceOwnerValidator<CustomResourceOwnerPasswordValidator>(); ;
        }
    }
}
