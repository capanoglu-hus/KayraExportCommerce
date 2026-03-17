using LogService.Workers;
using Serilog;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

// 2. IConnectionMultiplexer'ý Singleton olarak kaydet
// ConnectionMultiplexer.Connect() metodu aðýr bir iþlemdir, bu yüzden Singleton olmalý.
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// appsettings ayarlarýna göre yapýlandýracak
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
// redisten log almamk için
builder.Services.AddHostedService<LogWorker>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.UseSerilogRequestLogging(); // gelen isteklerin türünü  URL ve bilgilerini yazar

app.Run();
