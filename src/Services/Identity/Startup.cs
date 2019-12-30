// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using ConfigurationManager.Api.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer
{
    public class Startup
    {

        protected IConfiguration Configuration { get; }

        public IHostingEnvironment Environment { get; }

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        public IConfigurationSection GetSection(string sectionName)
        {
            return Configuration.GetSection(sectionName);
        }

        public void ConfigureServices(IServiceCollection services)
        {

            #region ElasticSearchLoggerConfigurations
            var elasticUri = Configuration["ElasticConfiguration:Uri"];
            var outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u4}] | {Message:l}{NewLine}{Exception}";

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "identityserver.cm.ahmetcagriakca.com")
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.File($"logs/identityserver-cm-ahmetcagriakca-log-{DateTime.Now:yyyy-MM-dd_HH:mm:ss}.log",
                    outputTemplate: outputTemplate,
                    fileSizeLimitBytes: 100_000_000,
                    rollOnFileSizeLimit: true)
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(
                        new Uri(elasticUri))
                    {
                        CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                        AutoRegisterTemplate = true,
                        TemplateName = "serilog-events-template",
                        IndexFormat = "identityserver-cm-ahmetcagriakca-log-{0:yyyy.MM.dd}"
                    })
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}{NewLine}", theme: AnsiConsoleTheme.Literate)
                .MinimumLevel.Verbose()
                .CreateLogger();
            Log.Information("Api started ...");
            #endregion
            var builder = services.AddIdentityServer(options =>
                {
                    options.Events.RaiseSuccessEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseErrorEvents = true;
                })
                .AddInMemoryIdentityResources(Config.GetIdentityResources())
                .AddInMemoryApiResources(Config.GetApis())
                .AddInMemoryClients(Config.GetClients())
                .AddCustomUserStore();

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                var identityConfig = Configuration.GetIdentityConfig("IdentityConfig");
                var certificate = new X509Certificate2(identityConfig.CertificatePath, identityConfig.CertificatePassword);
                builder.AddSigningCredential(certificate);
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Log.ForContext<Startup>()
                .Information("<{EventID:l}> Configure Started on {Env} {Application} with {@configuration}",
                    "Startup", Environment.EnvironmentName, Environment.ApplicationName, Configuration);
            app.UseIdentityServer();
        }
    }
}