using RedLockNet.SERedis.Configuration;

namespace connectors
{
    public class Configuration
    {
        public required RabbitMq RabbitMq { get; set; }
        public required string ElasticsearchUri { get; set; }
        public required List<RedLockEndPoint> RedisEndpoints { get; set; }
    }

    public class RedisEndpoint
    {
        public required string Host { get; set; }
        public required int Port { get; set; }
    }
    public class RabbitMq
    {
        public required string HostName { get; set; }
        public required string UserName { get; set; }
        public required string Password { get; set; }
        public required int Port { get; set; }
    }
}

