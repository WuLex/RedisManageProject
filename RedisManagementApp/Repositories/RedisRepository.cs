using CSRedis;
using Newtonsoft.Json;
using RedisManagementApp.Models;

namespace RedisManagementApp.Repositories
{
    public class RedisRepository<T> where T : class
    {
        private readonly CSRedisClient _redis;

        private readonly CommonConfig _commonConfig;

        //private readonly string _keyPrefix;

        public RedisRepository(CSRedisClient redisClient, CommonConfig commonConfig)
        {
            _redis = redisClient;
            _commonConfig = commonConfig;
        }

        public void Add(string key, T value)
        {
            var serializedValue = JsonConvert.SerializeObject(value);
            _redis.Set($"{_commonConfig.KeyPrefix}:{key}", serializedValue);
        }

        public T Get(string key)
        {
            var serializedValue = _redis.Get($"{_commonConfig.KeyPrefix}:{key}");
            if (serializedValue != null)
            {
                return JsonConvert.DeserializeObject<T>(serializedValue);
            }
            return null;
        }

        public void Update(string key, T value)
        {
            var serializedValue = JsonConvert.SerializeObject(value);
            _redis.Set($"{_commonConfig.KeyPrefix}:{key}", serializedValue);
        }

        public void Delete(string key)
        {
            _redis.Del($"{_commonConfig.KeyPrefix}:{key}");
        }

        public List<T> GetAll()
        {
            var keys = _redis.Keys($"{_commonConfig.KeyPrefix}:*");
            //var keys = _redis.Keys("*");
            var values = _redis.MGet(keys);
            var result = new List<T>();
            foreach (var value in values)
            {
                result.Add(JsonConvert.DeserializeObject<T>(value));
            }
            return result;
        }
    }
}
