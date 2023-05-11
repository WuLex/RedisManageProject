using CSRedis;

namespace RedisManagementApp.Utils
{
    public class RedisLuaUtil
    {
        private readonly CSRedisClient _redisClient;

        public RedisLuaUtil(CSRedisClient redisClient)
        {
            _redisClient = redisClient;
        }

        /// <summary>
        /// 扣减库存
        /// </summary>
        /// <param name="key"></param>
        /// <param name="delta"></param>
        /// <returns></returns>
        public async Task<bool> DecrStockAsync(string key, int delta)
        {
            string script = @"
                                local stock = tonumber(redis.call('get', KEYS[1]))
                                if stock == nil then
                                    return false
                                end
                                stock = stock - tonumber(ARGV[1])
                                if stock < 0 then
                                    return false
                                end
                                redis.call('set', KEYS[1], stock)
                                return true
                            ";
            var result = await _redisClient.EvalAsync(script, key, delta);
            return (bool)result;
        }

        /// <summary>
        /// 原子增加一个计数器
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> IncrCounterAsync(string key)
        {
            string script = @"
                                return redis.call('incr', KEYS[1])
                            ";
            var result = await _redisClient.EvalAsync(script, key);
            return (long)result;
        }

        /// <summary>
        /// 原子增加一个带有过期时间的计数器
        /// </summary>
        /// <param name="key"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public async Task<long> IncrCounterWithTtlAsync(string key, TimeSpan ttl)
        {
            string script = @"
                                local counter = redis.call('incr', KEYS[1])
                                if counter == 1 then
                                    redis.call('expire', KEYS[1], ARGV[1])
                                end
                                return counter
                            ";
            var result = await _redisClient.EvalAsync(script, key, (int)ttl.TotalSeconds);
            return (long)result;
        }

        /// <summary>
        /// 获取分布式锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="ttl"></param>
        /// <returns></returns>
        public async Task<bool> AcquireLockAsync(string key, string value, TimeSpan ttl)
        {
            string script = @"
                                local current_value = redis.call('get', KEYS[1])
                                if current_value == false or current_value == ARGV[1] then
                                    redis.call('set', KEYS[1], ARGV[1], 'PX', ARGV[2])
                                    return true
                                else
                                    return false
                                end
                            ";
            var result = await _redisClient.EvalAsync(script, key, value, (int)ttl.TotalMilliseconds);
            return (bool)result;
        }

        /// <summary>
        /// 释放分布式锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> ReleaseLockAsync(string key, string value)
        {
            string script = @"
                                if redis.call('get', KEYS[1]) == ARGV[1] then
                                    redis.call('del', KEYS[1])
                                    return true
                                else
                                    return false
                                end
                            ";
            var result = await _redisClient.EvalAsync(script, key, value);
            return (bool)result;
        }
    }
}