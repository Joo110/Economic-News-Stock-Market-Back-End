using BusinessLayer.Services;
using EconomicNews.Controllers;
using EconomicNews.Middlewares;
using EconomicNews_BLL;
using EconomicNews_BLL.Authentication;
using EconomicNews_BLL.Interfaces;
using EconomicNews_BLL.Saved;
using EconomicNews_BLL.Services;
using EconomicNews_BLL.StartupSeeders;
using EconomicNews_DAL;
using EconomicNews_DAL.Context;
using EconomicNews_DAL.Models;
using EconomicNews_DAL.Repositories;
using IntegrationLayer;
using IntegrationLayer.APIs;
using IntegrationLayer.DTOS;
using IntegrationLayer.Integrations;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ تسجيل DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ✅ تسجيل Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "EconomicNews_";
});

// ✅ تسجيل كل APIs
builder.Services.AddHttpClient<IntegrationLayer.Integrations.SectorApi>();
builder.Services.AddHttpClient<CompanyApi>();
builder.Services.AddHttpClient<CompanyStockApi>();
builder.Services.AddHttpClient<CryptoCurrencyIntegration>(client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "MyCryptoApp/1.0");
});
builder.Services.AddHttpClient<CurrencyIntegration>();
builder.Services.AddHttpClient<GoldPriceIntegration>();
builder.Services.AddHttpClient<HistoricalPriceApi>();
builder.Services.AddHttpClient<MarketStackService>();
builder.Services.AddHttpClient<MetalsCommodityIntegration>();
builder.Services.AddHttpClient<NewsIntegration>();
// ❌ شيلنا تسجيل Controller كـ HttpClient

// ✅ تسجيل Repositories
builder.Services.AddScoped<StockExchangeData>();
builder.Services.AddScoped<SectorRepository>();
builder.Services.AddScoped<CompanyRepository>();
builder.Services.AddScoped<CryptoCurrency>();
builder.Services.AddScoped<UserAlertData>();
builder.Services.AddScoped<CompanyStockReps>();
builder.Services.AddScoped<CurrencyData>();
builder.Services.AddScoped<GoldPriceData>();
builder.Services.AddScoped<HistoricalPriceRepository>();
builder.Services.AddScoped<MetaEnergyPriceData>();
builder.Services.AddScoped<NewsData>();
builder.Services.AddScoped<StockAnalysisRepository>();
builder.Services.AddScoped<UserData>();
builder.Services.AddScoped<UserPortfolioData>();
builder.Services.AddScoped<ErrorLogData>();

// ✅ تسجيل BLL Interfaces
builder.Services.AddScoped<IMetalsCommodityService, MetalsCommodityService>();
builder.Services.AddScoped<IGoldPriceService, GoldPriceService>();
builder.Services.AddScoped<INewsService, NewsBLL>();
builder.Services.AddScoped<ICurrencyService, CurrencyBLL>();

// ✅ تسجيل Business Logic Layer
builder.Services.AddScoped<StockExchangeRepository>();
builder.Services.AddScoped<EconomicNews_BLL.UserPortfolioService>();
builder.Services.AddScoped<CompanyBLL>();
builder.Services.AddScoped<StockAnalysisProcessor>();
builder.Services.AddScoped<HistoricalPriceService>();
builder.Services.AddScoped<CurrencyBLL>();
builder.Services.AddScoped<GoldPrice>();
builder.Services.AddScoped<CryptoCurrencyService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<MetalsCommodityService>();
builder.Services.AddScoped<NewsBLL>();
builder.Services.AddScoped<User>();
builder.Services.AddScoped<UserAlertBLL>();
builder.Services.AddScoped<SectorBLL>();
builder.Services.AddScoped<JwtTokenService>();

// ✅ خدمات إضافية
builder.Services.AddScoped<SavedDataFromStock>();
builder.Services.AddScoped<StartupSeeder>();

// ✅ Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // ✅ إضافة تعريف للـ Bearer Token
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EconomicNews API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "ادخل التوكن هنا (Bearer JWT)\n مثال: Bearer {your token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// ===== إضافة إعدادات CORS =====
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ✅ إعداد JWT
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Key)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// ===== بناء التطبيق =====
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// استخدم CORS قبل المصادقة
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseMiddleware<GlobalErrorMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seeder (جرّب تعطلها الأول لو Swagger لسه بيوقع)
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<StartupSeeder>();
    await seeder.SeedAsync();
}

app.Run();
