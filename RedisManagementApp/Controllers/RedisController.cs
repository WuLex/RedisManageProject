using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RedisManagementApp.Models;
using RedisManagementApp.Repositories;
using RedisManagementApp.Utils;

namespace RedisManagementApp.Controllers
{
    public class RedisController : Controller
    {
        private readonly RedisRepository<MyDataModel> _redisRepository;
        private readonly IConfiguration _configuration;
        private readonly CommonConfig _commonConfig;


        public RedisController(IConfiguration config, RedisRepository<MyDataModel> redisRepository, CommonConfig commonConfig)
        {
            //var connectionString = config.GetConnectionString("Redis");
            
            _redisRepository = redisRepository;
            _commonConfig = commonConfig;
            _commonConfig.KeyPrefix = "mng";
        }

        public IActionResult Index()
        {
            var data = _redisRepository.GetAll();
            return View(data);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(MyDataModel model)
        {
            model.Id = new IdWorker(1, 1).NextId().ToString();
            try
            {
                _redisRepository.Add(model.Id, model);
                return Json(new { success = true, msg = "添加成功" });
            }
            catch (Exception)
            {
                return Json(new { success = false, msg = "添加失败" });
            }
          
            //return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string id)
        {
            var model = _redisRepository.Get(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(string id, MyDataModel model)
        {
            try
            {
                _redisRepository.Update(id, model);
                return Json(new { success = true, msg = "更新成功" });
            }
            catch (Exception)
            {
                return Json(new { success = false, msg = "更新失败" });
            }


            //return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            try
            {
                _redisRepository.Delete(id);

                return Json(new { success = true, msg = "删除成功" }); 
            }
            catch (Exception)
            {
                return Json(new { success = false, msg = "删除失败" });
            }
        
            //return RedirectToAction(nameof(Index));
        }
    }
}