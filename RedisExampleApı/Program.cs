
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using RedisExampleApı.Cache;
using RedisExampleApı.Models;
using RedisExampleApı.Repository;
using StackExchange.Redis;
using IDatabase = StackExchange.Redis.IDatabase;

namespace RedisExampleApı
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //IProductRepository and RedisService = If there is data in the redist it will be used
            builder.Services.AddScoped<IProductRepository>(sp =>
            {
                var appDbContext=sp.GetRequiredService<AppDbContext>();
                var productRepository=new ProductRepository(appDbContext);
                var redisService = sp.GetRequiredService<RedisService>();
                return new ProductRepositoryWithCacheDecorater(productRepository, redisService);
            });

            //IProductRepository and ProductRepository = If there is no data in the redist, it will be used
            builder.Services.AddScoped<IProductRepository, ProductRepository>();

            //db
            builder.Services.AddDbContext<AppDbContext>(opt =>
            {
                opt.UseInMemoryDatabase("mydatabase");
            });

            //RedisServiceyi bağlamak 
            builder.Services.AddSingleton<RedisService>(sp =>
            {
                //construcctor olduğu için ve url istediği için yaptık 
                return new RedisService(builder.Configuration["CacheOptions:Url"]);
            });

            ////db yi burada da alaniliriz amaç controllerda hep çağırmamak
            //builder.Services.AddSingleton<IDatabase>(sp =>
            //{
            //    var redisService=sp.GetRequiredService<RedisService>();
            //    return redisService.GetDb(0);
            //});

            var app = builder.Build();

            //ınmemory old için veriler gözükmüyor sorun çözümü 
            using(var scope = app.Services.CreateScope())
            {
                var dbContext=scope.ServiceProvider.GetRequiredService<AppDbContext>();
                dbContext.Database.EnsureCreated();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}