using CSRedis;

namespace RedisManagementApp.Common
{
    public class RedisStockUpdater
    {
        private readonly CSRedisClient _redis;

        //public RedisStockUpdater(string connectionString)
        //{
        //    _redis = new CSRedisClient(connectionString);
        //}

        public RedisStockUpdater(CSRedisClient redis)
        {
            _redis = redis;
        }

        public async Task<bool> UpdateAsync(string key, int delta)
        {
            string script = @"
            local stock = tonumber(redis.call('get', KEYS[1]))
            if stock == nil then
                return false
            end
            stock = stock + tonumber(ARGV[1])
            if stock < 0 then
                return false
            end
            redis.call('set', KEYS[1], stock)
            return true
        ";
            var result = await _redis.EvalAsync(script, key, delta);
            return (bool)result;
        }
    }
}
