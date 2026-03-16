using Microsoft.OpenApi;
using ProductionService.Application.CQRSDesignPattern.Handlers;
using ProductService.Persistence.Context;

var builder = WebApplication.CreateBuilder(args);
/*veritabaný baðlantýsý ekleme*/
builder.Services.AddDbContext<ProductionServiceContext>();
// Add services to the container.

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
