using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// limit ayarlarý
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixed-policy", opt =>
    {
        opt.Window = TimeSpan.FromSeconds(10); // 10 sn bekler
        opt.PermitLimit = 5; //en fazla 5 istek süre zarfýnda
        opt.QueueLimit = 2; //2 kuyruða alýr 
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });
    // çok fazla istek gelirse 429 hatasý
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});
// merkezi yerden proxy 
builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
// token kontrolu için jwt ayarlarý
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
            ClockSkew = TimeSpan.Zero // saat kaymasý
        };
    });
builder.Services.AddAuthorization();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthentication(); 
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseRateLimiter(); // limit için 

app.MapReverseProxy(); //proxy düzenlemesi için

app.Run();
