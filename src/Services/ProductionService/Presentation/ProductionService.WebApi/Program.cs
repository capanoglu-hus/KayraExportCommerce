using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ProductionService.Application.CQRSDesignPattern.Handlers;
using ProductionService.Persistence;

using ProductionService.Persistence.Context;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/*veritabaný baðlantýsý ekleme*/
builder.Services.AddDbContext<ProductionServiceContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5,
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorNumbersToAdd: null))

);




/* redis için baðlantý alýmý*/
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");
// redis baðlantýsý yapma
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

/* apigateway token gönderimini service içinde kontrol etmek*/
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecurityKey"])),
            ClockSkew = TimeSpan.Zero 
        };
    });

// Authorization -> iþlemini için  
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));


// Cache servisini 
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetProductQueryHandler).Assembly));
/* direkt IRequestHandler sýnýfý kaydediyor assembly de */

builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddEndpointsApiExplorer();
/*Swagger yapýsýný kullanmak için */
builder.Services.AddSwaggerGen(x =>
{
    x.SwaggerDoc("v1", new OpenApiInfo { Title = "My Api", Version = "v1" });
});

var app = builder.Build();
// eðer sqlserver da daha öce db oluþmamýþsa migrationlarý içeri aktaracak
 using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ProductionServiceContext>();

        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Veritabaný migration sýrasýnda bir hata oluþtu.");
    }
} 

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

app.UseAuthentication();  
app.UseAuthorization(); 

app.MapControllers();

app.Run();
