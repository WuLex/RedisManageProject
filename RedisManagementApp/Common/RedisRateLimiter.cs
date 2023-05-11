using CSRedis;

namespace RedisManagementApp.Common
{
    public class RedisRateLimiter
    {
        private readonly CSRedisClient _redisClient;

        //public RedisRateLimiter(string connectionString)
        //{
        //    _redisClient = new CSRedisClient(connectionString);
        //}
        public RedisRateLimiter(CSRedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        public async Task<bool> LimitAsync(string key, int limit, int window)
        {
            var luaScript = @"
            local key = KEYS[1]
            local limit = tonumber(ARGV[1])
            local window = tonumber(ARGV[2])
            local now = tonumber(redis.call('time')[1])
            local count = tonumber(redis.call('get', key) or '0')
            if count < limit then
                redis.call('incr', key)
                redis.call('expire', key, window)
                return true
            else
                return false
            end";
            var result = await _redisClient.EvalAsync(luaScript, key, limit, window);
            return (bool)result;
        }
    }
}
