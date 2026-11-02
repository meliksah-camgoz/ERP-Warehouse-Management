
using Microsoft.EntityFrameworkCore;
using MiniERP.Infrastructure.Persistence;

using MiniERP.Application.Categories;
using MiniERP.Infrastructure.Categories;

using MiniERP.Application.Products;
using MiniERP.Infrastructure.Products;

using MiniERP.Application.Warehouses;
using MiniERP.Infrastructure.Warehouses;

using MiniERP.Application.StockBalances;
using MiniERP.Infrastructure.StockBalances;

using MiniERP.Application.Assets;
using MiniERP.Infrastructure.Assets;

using MiniERP.Application.Authentication;
using MiniERP.Infrastructure.Authentication;

using MiniERP.Domain.Entities;

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// CORS
// --------------------------------------------------

builder.Services.AddCors(options =>
{
    options.AddPolicy("Angular", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:4200",
                "http://192.168.1.91:4200"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// --------------------------------------------------
// JWT Authentication
// --------------------------------------------------

var jwtSettings = builder.Configuration.GetSection("Jwt");

var jwtKey = jwtSettings["Key"]
    ?? throw new InvalidOperationException("JWT Key ayarı bulunamadı.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            ),

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

builder.Services.AddAuthorization();

// --------------------------------------------------
// Dependency Injection
// --------------------------------------------------

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IWarehouseService, WarehouseService>();
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<IStockBalanceService, StockBalanceService>();
builder.Services.AddScoped<IStockBalanceRepository, StockBalanceRepository>();

builder.Services.AddScoped<IAssetService, AssetService>();
builder.Services.AddScoped<IAssetRepository, AssetRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();

// --------------------------------------------------
// Database
// --------------------------------------------------

builder.Services.AddDbContext<MiniERPDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("Default")
    )
);

// --------------------------------------------------
// Controllers and Swagger
// --------------------------------------------------

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --------------------------------------------------
// Build Application
// --------------------------------------------------

var app = builder.Build();

// --------------------------------------------------
// Angular Static Files
// --------------------------------------------------

app.UseDefaultFiles();
app.UseStaticFiles();

// --------------------------------------------------
// Development Database Initialization
// --------------------------------------------------

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var db = scope.ServiceProvider
        .GetRequiredService<MiniERPDbContext>();

    var username = "bilgiislem";

    if (!db.AppUsers.Any(u => u.Username == username))
    {
        var user = new AppUser
        {
            Username = username,
            Role = "Admin",
            IsActive = true
        };

        var hasher = new PasswordHasher<AppUser>();

        user.PasswordHash = hasher.HashPassword(
            user,
            "3470"
        );

        db.AppUsers.Add(user);
        db.SaveChanges();
    }
}

// --------------------------------------------------
// Swagger
// --------------------------------------------------

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// --------------------------------------------------
// HTTP Pipeline
// --------------------------------------------------

app.UseCors("Angular");

app.UseAuthentication();
app.UseAuthorization();

// --------------------------------------------------
// API Endpoints
// --------------------------------------------------

app.MapControllers();

// Health Check
app.MapGet("/health", () => Results.Ok(new
{
    application = "MiniERP API",
    status = "Running"
}));

// Angular Routing Fallback
app.MapFallbackToFile("index.html");

// --------------------------------------------------
// Run
// --------------------------------------------------

app.Run();
