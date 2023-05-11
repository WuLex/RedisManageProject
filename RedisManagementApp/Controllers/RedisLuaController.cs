using Microsoft.AspNetCore.Mvc;
using RedisManagementApp.Common;
using RedisManagementApp.Models;

namespace RedisManagementApp.Controllers
{
    public class RedisLuaController : ControllerBase
    {
        private readonly RedisRateLimiter _redisRateLimiter;
        private readonly RedisStockUpdater _redisStockUpdater;

        public RedisLuaController(RedisRateLimiter redisRateLimiter, RedisStockUpdater redisStockUpdater)
        {
            _redisRateLimiter = redisRateLimiter;
            _redisStockUpdater = redisStockUpdater;
        }


        [HttpPost("/api/order")]
        public async Task<IActionResult> PlaceOrder([FromBody] Order order)
        {
            // 检查限流
            bool canAccess = await _redisRateLimiter.LimitAsync("api:access", 10, 60);
            if (!canAccess)
            {
                return StatusCode(429); // 返回 HTTP 429 Too Many Requests 状态码
            }

            // 扣减库存
            bool success = await _redisStockUpdater.UpdateAsync("stock:product1", -order.Quantity);
            if (!success)
            {
                return StatusCode(503); // 返回 HTTP 503 Service Unavailable 状态码
            }

            // 处理订单
            // ...

            return Ok();
        }
    }
}
