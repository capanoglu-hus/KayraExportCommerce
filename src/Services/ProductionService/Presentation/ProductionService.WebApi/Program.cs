using Microsoft.OpenApi;
using ProductionService.Application.CQRSDesignPattern.Handlers;
using ProductionService.Persistence;

using ProductionService.Persistence.Context;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
/*veritabaný baðlantýsý ekleme*/
builder.Services.AddDbContext<ProductionServiceContext>();
// Add services to the container.

// 1. Baðlantý dizesini al (appsettings.json'dan)
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

// 2. IConnectionMultiplexer'ý Singleton olarak kaydet
// ConnectionMultiplexer.Connect() metodu aðýr bir iþlemdir, bu yüzden Singleton olmalý.
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

// 3. Kendi Cache servisini kaydet
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetProductQueryHandler).Assembly));
/* direkt IRequestHandler sýnýfý kaydediyor assembly de */
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
/*Swagger yapýsýný kullanmak için */
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "My Api", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Api v1");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
