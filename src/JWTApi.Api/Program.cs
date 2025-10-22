//using JwtApi.Api.Middleware;
//using JWTApi.Api.Middleware;
//using JWTApi.Application.Services;
//using JWTApi.Domain.Entities;
//using JWTApi.Domain.Interfaces;
//using JWTApi.Infrastructure.Data;
//using JWTApi.Infrastructure.Middleware;
//using JWTApi.Infrastructure.Repositories;
//using JWTApi.Infrastructure.Services;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.Text;

//var builder = WebApplication.CreateBuilder(args);

//// DB
//builder.Services.AddDbContext<AppDbContext>(opt =>
//    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// DI
//builder.Services.AddMemoryCache();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
//builder.Services.AddScoped<ITodoStatus, TodoStatusRepository>();
//builder.Services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
//builder.Services.AddScoped<JwtService>();
//builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<ProjectService>();
//builder.Services.AddScoped<TodoStatusService>();
//builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<User>, Microsoft.AspNetCore.Identity.PasswordHasher<User>>();
//builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection("Security"));

//// register token blacklist (replace with Redis implementation in production)
//builder.Services.AddSingleton<ITokenBlacklist, InMemoryTokenBlacklist>();
//// JWT
//var jwt = builder.Configuration.GetSection("Jwt");
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(opt =>
//    {
//        opt.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = jwt["Issuer"],
//            ValidAudience = jwt["Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
//            ClockSkew=TimeSpan.Zero
//        };
//    });
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowReactApp", policy =>
//    {
//        policy.WithOrigins("https://localhost:3000", "http://localhost:3000")
//              .AllowAnyHeader()
//              .AllowAnyMethod()
//              .AllowCredentials();
//    });
//});

//// Controllers & Swagger
//builder.Services.AddControllers();
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Title = "JWT Auth API",
//        Version = "v1",
//        Description = "API for JWT Authentication with Refresh Token"
//    });
//});

//var app = builder.Build();

//// Enable Swagger for all environments (or keep only Development)
//app.UseSwagger();
//app.UseSwaggerUI(c =>
//{
//    c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT Auth API V1");
//});

//app.UseApiSecurity();
//app.UseCors("AllowReactApp");
////app.UseRateLimiter();
//// ثبت IMemoryCache

//app.UseHttpsRedirection();
//app.UseAuthentication();
//app.UseStaticFiles();
//app.UseAuthorization();
////app.UseCustomRateLimiter();
//app.UseCustomExceptionHandler();
////app.UseMiddleware<RateLimitMiddleware>();
////app.UseMiddleware<SecurityMiddleware>();
//app.UseMiddleware<MenuPermissionMiddleware>();
//app.UseMiddleware<ExceptionHandlingMiddleware>();
//app.MapControllers();
//app.Run();

using JwtApi.Api.Middleware;
using JWTApi.Api.Middleware;
using JWTApi.Application.Services;
using JWTApi.Domain.Entities;
using JWTApi.Domain.Interfaces;
using JWTApi.Infrastructure.Data;
using JWTApi.Infrastructure.Repositories;
using JWTApi.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CORS Configuration - اضافه کردن این قسمت در ابتدا
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("https://localhost:3000", "http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .WithExposedHeaders("X-Pagination", "Content-Disposition") // در صورت نیاز
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10)); // کش کردن preflight
    });

    // برای محیط production
    options.AddPolicy("AllowProduction", policy =>
    {
        policy.WithOrigins("https://yourdomain.com", "https://www.yourdomain.com")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// DI
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<ITodoStatus, TodoStatusRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWorkRepository>();
builder.Services.AddScoped<IBaleRepository, BaleRepository>();
builder.Services.AddScoped<ITodo, TodoRepository>();
builder.Services.AddScoped<TodoService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<BaleService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<TodoStatusService>();
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.IPasswordHasher<User>, Microsoft.AspNetCore.Identity.PasswordHasher<User>>();
builder.Services.Configure<SecurityOptions>(builder.Configuration.GetSection("Security"));

// register token blacklist
builder.Services.AddSingleton<ITokenBlacklist, InMemoryTokenBlacklist>();

// JWT
var jwt = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!)),
            ClockSkew = TimeSpan.Zero
        };
    });

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JWT Auth API",
        Version = "v1",
        Description = "API for JWT Authentication with Refresh Token"
    });
});
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping; // برای کاراکترهای خاص
    });
builder.Services.AddHttpClient("BaleClient", client =>
{
    client.BaseAddress = new Uri("https://tapi.bale.ai/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Middleware pipeline - ترتیب بسیار مهم است
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "JWT Auth API V1");
    });
}

app.UseHttpsRedirection();

// CORS باید قبل از Authentication و Authorization باشد
app.UseCors("AllowReactApp"); // این خط باید دقیقاً اینجا باشد

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();

// سایر middlewareهای سفارشی شما
app.UseApiSecurity();
app.UseCustomExceptionHandler();
app.UseMiddleware<MenuPermissionMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();
app.Run();
