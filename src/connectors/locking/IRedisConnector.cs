using System;
using RedLockNet.SERedis;

namespace connectors.locking
{
	public interface IRedisConnector
	{
		RedLockFactory CreateRedLockFactory();
	
	}
}

