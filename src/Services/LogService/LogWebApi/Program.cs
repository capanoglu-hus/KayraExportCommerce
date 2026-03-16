using LogService.Workers;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

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
