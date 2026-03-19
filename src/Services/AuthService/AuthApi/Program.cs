using AuthApi.Data;
using AuthApi.Models;
using AuthApi.Services;
using AuthApi.Worker;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// redis için baðlantý yapýsý
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

// multiconnect olarak baðlanma redise
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(redisConnectionString));

// servisleri alýyor
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IRedisService, RedisService>();

// event için worker
builder.Services.AddHostedService<EventSubscriber>();

builder.Services.AddMemoryCache(); 

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// veritabaný baðlantýsý
builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure(
        maxRetryCount: 5, 
        maxRetryDelay: TimeSpan.FromSeconds(10),
        errorNumbersToAdd: null))

);

// register-login kurallarý
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<UserDbContext>()
    .AddDefaultTokenProviders();

// yetkilendirme iþleminde direkt jwt kontrolu
// jwt token özelliklerinin tek tek doðrulanmasý
var jwt = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwt["SecurityKey"]);
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero // saat kaymasý
        };
    });

builder.Services.AddScoped<ITokenService, TokenService>();
var app = builder.Build();
// sqlserver docker da ilk kurulmasý için
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<UserDbContext>();

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
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication(); // yetkilendirme
app.UseAuthorization();

app.MapControllers();

app.Run();
