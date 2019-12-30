using Autofac;
using Autofac.Extensions.DependencyInjection;
using ConfigurationManager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using ConfigurationManager.Core.Data;
using ConfigurationManager.Core.Model;

namespace ConfigurationManager.Api.Utility
{
    public static class ServiceCollectionExtension
    {
        public static IServiceProvider BuildProvider<T>(this IMvcBuilder mvcBuilder, Action<IServiceCollection> services = null) where T : AutofacServiceProvider
        {
            return ServiceProviderFactory.Create(mvcBuilder, services);
        }
    }

    public static class ServiceProviderFactory
    {
        public static AutofacServiceProvider Create(IMvcBuilder mvcBuilder, Action<IServiceCollection> action)
        {
            if (action != null)
                action.Invoke(mvcBuilder.Services);
            return new AutofacServiceProvider(CreateComponentContext(mvcBuilder));
        }

        private static ILifetimeScope CreateComponentContext(IMvcBuilder mvcBuilder)
        {
            var shellContainer = Build();
            var workScope = CreateWorkScope(mvcBuilder, shellContainer);
            return workScope;
        }

        private static ILifetimeScope CreateWorkScope(IMvcBuilder mvcBuilder, IContainer shellContainer)
        {
            return shellContainer.BeginLifetimeScope(builder =>
            {
                builder.Populate(mvcBuilder.Services);
            });
        }

        public static IContainer Build(Action<ContainerBuilder> registration = null)
        {
            var builder = new ContainerBuilder();
            {
                builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>();
                builder.RegisterType<DbContextLocator>().As<IMongoDbContextLocator>();
                builder.RegisterGeneric(typeof(MongoRepositorySequence<>)).As(typeof(IMongoRepositorySequence<>));
                builder.RegisterGeneric(typeof(MongoRepositoryEvent<>)).As(typeof(IMongoRepositoryEventHandler<>));
                builder.RegisterGeneric(typeof(MongoRepository<>)).As(typeof(IMongoRepository<>));
                builder.RegisterType<ApplicationConfigurationRepository>().As<IApplicationConfigurationRepository>();
                builder.RegisterType<ResultBuilder>().As<IActionResultBuilder>();
                

                builder.RegisterType<MongoDbContext>();
                if (registration != null)
                {
                    registration.Invoke(builder);
                }
            }
            return builder.Build();
        }
    }
}
