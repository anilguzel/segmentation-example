using connectors.locking;
using RedLockNet;

namespace services.locking
{
    public class RedisLockService : IDistributedLockService
	{
        private readonly IRedisConnector _redisConnector;

        public RedisLockService(IRedisConnector redisConnector)
        {
            _redisConnector = redisConnector;
        }

        public async Task<IRedLock> CreateLockAsync(string resourceName, TimeSpan? expiryTime = null)
        {
            var redLockFactory = _redisConnector.CreateRedLockFactory();

            expiryTime ??= TimeSpan.FromSeconds(15);

            var a = await redLockFactory.CreateLockAsync(resourceName, expiryTime.Value);
            return a;
        }
    }
}

