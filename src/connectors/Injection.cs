using connectors.locking;
using Microsoft.Extensions.DependencyInjection;

public static class Injection
{
    public static void AddConnectors(this IServiceCollection services, connectors.Configuration configuration)
    {
        services.AddSingleton<IRabbitMqConnector>(_ => new RabbitMqConnector(configuration.RabbitMq.HostName, configuration.RabbitMq.UserName, configuration.RabbitMq.Password, configuration.RabbitMq.Port));

        services.AddSingleton<IElasticsearchConnector>(_ => new ElasticsearchConnector(configuration.ElasticsearchUri));

        services.AddSingleton<IRedisConnector>(_ => new RedisConnector(configuration.RedisEndpoints));
    }
}
