using LogService.Workers;
using Serilog;
using Serilog.Context;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// redis baðlantýsý ayarlarý
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

// redis baðlantýsý baþ.
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// appsettings ayarlarýna göre yapýlandýracak
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration) // appsettings okur
    .ReadFrom.Services(services) // servisleri okur
    .Enrich.FromLogContext());

// redisten log almamk için - worker
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
