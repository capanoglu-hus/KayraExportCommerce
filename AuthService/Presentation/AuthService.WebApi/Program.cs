using AuthService.Application.CQRSDesign.Handler;
using AuthService.Application.Helpers;
using AuthService.Application.Services.Abstract;
using AuthService.Application.Services.Concrate;
using AuthService.Persistence.Context;
using AuthService.Persistence.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AuthContext>();
// Add services to the container.
builder.Services.AddScoped<CreateUserRegisterCommandHandler>();
builder.Services.AddScoped<IAuthjwtService, AuthjwtService>();
builder.Services.AddScoped<TokenHelpers>();
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AuthContext>();
builder.Services.AddAuthentication(opt =>
{
    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
//builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetProductQueryHandler).Assembly));
/* direkt IRequestHandler sýnýfý kaydediyor assembly de */
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
