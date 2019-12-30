using Autofac.Extensions.DependencyInjection;
using ConfigurationManager.Api.Model;
using ConfigurationManager.Api.Utility;
using ConfigurationManager.Api.Utility.Filters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using Serilog.Formatting.Elasticsearch;
using Serilog.Sinks.Elasticsearch;

namespace ConfigurationManager.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {

            #region ElasticSearchLoggerConfigurations
            var elasticUri = Configuration["ElasticConfiguration:Uri"];

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "cm.ahmetcagriakca.com")
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .WriteTo.Elasticsearch(
                    new ElasticsearchSinkOptions(
                        new Uri(elasticUri))
                    {
                        CustomFormatter = new ExceptionAsObjectJsonFormatter(renderMessage: true),
                        AutoRegisterTemplate = true,
                        TemplateName = "serilog-events-template",
                        IndexFormat = "cm-ahmetcagriakca-log-{0:yyyy.MM.dd}"
                    })
                .MinimumLevel.Verbose()
                .CreateLogger();
            Log.Information("Api started ...");
            #endregion
            //Getting identity config
            return services
                        .AddIdentityServer(Configuration.GetIdentityConfig("IdentityConfig"))
                        .AddCors(o => o.AddPolicy("AllowAll", builder =>
                        {
                            builder.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                        }))
                        .AddSwagger(Configuration.GetIdentityConfig("IdentityConfig"))
                        .AddConfigurationManager(Configuration.GetConfigurationConfig("ConfigurationEnvironment"))
                        .AddMongoDbContext(Configuration.GetConfigurationConfig("ConfigurationEnvironment"))
                        .AddMvc()
                        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                        .AddMvcOptions(options =>
                        {
                            options.Filters.Add<LoggingActionFilter>(int.MinValue);
                            options.Filters.Add<ConfigurationManagerExceptionFilter>();

                        })
                        .AddJsonOptions(x =>
                        {
                            x.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                            x.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local;
                        })
                        .BuildProvider<AutofacServiceProvider>(); ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            Log.ForContext<Startup>()
                .Information("<{EventID:l}> Configure Started on {Env} {Application} with {@configuration}",
                    "Startup", env.EnvironmentName, env.ApplicationName, Configuration);
            loggerFactory.AddSerilog();
            app.UseCors("AllowAll")
                .UseMvc()
                .UseSwagger()
                .UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Configuration Manager Api V1");
                    options.OAuthClientId("client");
                    options.OAuthClientSecret("secret");
                    options.RoutePrefix = string.Empty;
                });
        }
    }
}
