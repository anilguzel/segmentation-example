using Microsoft.Extensions.DependencyInjection;
using services.locking;

public static class Injection
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IElasticsearchService, ElasticsearchService>();
        services.AddSingleton<IRabbitMqService, RabbitMqService>();
        services.AddSingleton<IDistributedLockService, RedisLockService>();
    }
}
