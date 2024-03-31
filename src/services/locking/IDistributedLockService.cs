using System;
using RedLockNet;

namespace services.locking
{
	public interface IDistributedLockService
	{
        Task<IRedLock> CreateLockAsync(string resourceName, TimeSpan? expiryTime = null);
    }
}

