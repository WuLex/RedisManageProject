using CSRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RedisManagementApp.Common;
using RedisManagementApp.Models;
using RedisManagementApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
// 注册 CSRedisClient 对象
builder.Services.AddSingleton<CSRedisClient>(c => new CSRedisClient(Configuration.GetConnectionString("Redis")));

// Register RedisConfiguration as a Singleton service
builder.Services.AddSingleton(new CommonConfig { KeyPrefix = "defaultPrefix" });


var redisConnectionString = Configuration.GetConnectionString("Redis");

// 注册 RedisRateLimiter 对象
builder.Services.AddScoped<RedisRateLimiter>();
//builder.Services.AddScoped<RedisRateLimiter>(_ => new RedisRateLimiter(redisConnectionString));

// 注册 RedisStockUpdater 对象
builder.Services.AddScoped<RedisStockUpdater>();
//builder.Services.AddScoped<RedisStockUpdater>(_ => new RedisStockUpdater(redisConnectionString));

// Add RedisRepository<T> as a Scoped service
builder.Services.AddScoped(typeof(RedisRepository<>));


// Add services to the container.
builder.Services.AddControllersWithViews();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
