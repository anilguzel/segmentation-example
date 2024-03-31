using System.Net;
using RedLockNet.SERedis;
using RedLockNet.SERedis.Configuration;

namespace connectors.locking
{
    public class RedisConnector : IRedisConnector
    {
        private readonly List<RedLockEndPoint> EndPoints;

        public RedisConnector(List<RedLockEndPoint> endPoints)
        {
            EndPoints = endPoints;
        }

        public RedLockFactory CreateRedLockFactory()
        {
            var a = RedLockFactory.Create(EndPoints);
            return a;
        }
    }
}

