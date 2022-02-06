using app.consumer.impl;
using app.consumer.interfaces;
using config.broker.RabbitMQ.impl;
using config.broker.RabbitMQ.interfaces;
using config.database.PostgresSQL.impl;
using config.database.PostgresSQL.interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace app.DI
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveAppDependencies(this IServiceCollection services)
        {
            services.AddScoped<IRegistrationInformationConsumer, RegistrationInformationConsumer>();
            return services;
        }

        public static IServiceCollection ResolveDomainDependencies(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection ResolveGatewayDependencies(this IServiceCollection services)
        {
            return services;
        }

        public static IServiceCollection ResolveConfigDependencies(this IServiceCollection services)
        {
            services.AddScoped<IRabbitMQConnection, RabbitMQConnection>();
            services.AddScoped<IPostgreSQLHelper, PostgreSQLHelper>();
            services.AddScoped<IReceiver, Receiver>();
            services.AddScoped<ISender, Sender>();
            return services;
        }
    }
}