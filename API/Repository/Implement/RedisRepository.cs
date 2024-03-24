using API.Data;
using Medallion.Threading.Redis;
using StackExchange.Redis;

namespace API.Repository.Implement
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase _redisDb;
        private readonly StoreContext _storeContext;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IConnectionMultiplexer _redis;

        public RedisRepository(IConnectionMultiplexer redis, StoreContext storeContext, IInventoryRepository inventoryRepository, ILogger<RedisRepository> logger)
        {
            _redis = redis;
            _redisDb = redis.GetDatabase();
            _storeContext = storeContext;
            _inventoryRepository = inventoryRepository;

            var endpoints = _redisDb.Multiplexer.GetEndPoints();
            var server = _redisDb.Multiplexer.GetServer(endpoints.First());
            Console.WriteLine($"Connected to Redis server: {server.EndPoint}");

        }
        public async Task<string> AcquireLockAsync(int productId, int quantity, int cartId)
        {
            string key = $"lock_v2024_{productId}";
            int retryTimes = 10;
            TimeSpan expiry = TimeSpan.FromMilliseconds(3000); // 3 seconds

            for (int i = 0; i < retryTimes; i++)
            {
                var @lock = new RedisDistributedLock(key, _redisDb);
                {
                    await using (var handle = await @lock.TryAcquireAsync())
                        if (handle != null)
                        {
                            int modifiedCount = await _inventoryRepository.ReservationInventory(productId, quantity, cartId);
                            if (modifiedCount > 0)
                            {
                                return key;
                            }
                        }
                        else
                        {
                            Console.WriteLine($"Unable to acquire lock for product {productId}. Retrying...");
                            await Task.Delay(50);
                        }
                }
            }
            Console.WriteLine($"Failed to acquire lock for product {productId} after {retryTimes} attempts.");
            return null;
        }
    }
}
