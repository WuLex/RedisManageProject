using CSRedis;
using Microsoft.Extensions.Configuration;
using RedisManagementApp.Models;
using RedisManagementApp.Repositories;

var builder = WebApplication.CreateBuilder(args);

var Configuration = builder.Configuration;
// Add Redis as a Singleton service
builder.Services.AddSingleton<CSRedisClient>(c => new CSRedisClient(Configuration.GetConnectionString("Redis")));

// Register RedisConfiguration as a Singleton service
builder.Services.AddSingleton(new CommonConfig { KeyPrefix = "defaultPrefix" });


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
