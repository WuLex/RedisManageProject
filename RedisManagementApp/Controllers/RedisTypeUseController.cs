using CSRedis;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Intrinsics.Arm;
using System.Threading.Channels;

namespace RedisManagementApp.Controllers
{
    public class RedisTypeUseController : Controller
    {
        private readonly CSRedisClient _redisClient;
        public RedisTypeUseController(CSRedisClient redisClient)
        { 
            // 创建Redis客户端
            _redisClient = redisClient;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Subscribe()
        {
            //普通订阅
            //_redisClient.Subscribe(
            //  ("chan1", msg => Console.WriteLine(msg.Body)),
            //  ("chan2", msg => Console.WriteLine(msg.Body)));

            ////模式订阅（通配符）
            //_redisClient.PSubscribe(new[] { "test*", "*test001", "test*002" }, msg => {
            //    Console.WriteLine($"PSUB   {msg.MessageId}:{msg.Body}    {msg.Pattern}: chan:{msg.Channel}");
            //});
            //模式订阅已经解决的难题：
            //1、分区的节点匹配规则，导致通配符最大可能匹配全部节点，所以全部节点都要订阅
            //2、本组 "test*", "*test001", "test*002" 订阅全部节点时，需要解决同一条消息不可执行多次

          
            var result=string.Empty;
            // 订阅一个频道
            _redisClient.Subscribe(("mychannel",message =>
            {
                 result = $"收到来自频道 {message.Channel} 的消息：{message}";
            }));

            //同上
            //RedisHelper.PSubscribe(new[] { "mychannel" }, message =>
            //{
            //    Console.WriteLine($"收到来自频道 {message.Channel} 的消息：{message}");
            //});

            Console.WriteLine("已订阅 mychannel 频道，按任意键取消订阅并退出程序...");
            Console.ReadKey();

            return Content(result);
        }

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public IActionResult Publish(string channel, string message)
        {
            _redisClient.Publish(channel, message);

            return Ok();
        }
        

    }
}
