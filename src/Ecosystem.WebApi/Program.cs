using Asp.Versioning;
using Ecosystem.Application;
using Ecosystem.Infrastructure;
using Ecosystem.Persistence.ReadDb;
using Ecosystem.Persistence.WriteDb;
using Ecosystem.WebApi.Middleware;
using Ecosystem.WebApi.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Cấu hình API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Cấu hình API Documentation (Scalar/OpenAPI)
builder.Services.AddEndpointsApiExplorer();

// Đăng ký dịch vụ tạo OpenAPI và thêm vào transformer tùy chỉnh của chúng ta
builder.Services.AddOpenApi(options =>
{
    // Dòng này sẽ tự động gọi BearerSecuritySchemeTransformer
    options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
});

// Add services to the container.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddReadPersistence(builder.Configuration);

// Thêm dịch vụ cho Controllers để hệ thống nhận diện và sử dụng các API Controller
builder.Services.AddControllers();

// Cấu hình CORS cho phép Angular frontend truy cập
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Cấu hình dịch vụ xác thực JWT Bearer
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Đọc cấu hình từ appsettings.json
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
    });

// Cấu hình dịch vụ ủy quyền (Authorization)
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
    app.MapGet("/", () => Results.Redirect("/scalar"));
}

// Sử dụng Middleware xử lý lỗi toàn cục mà chúng ta đã tạo
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

// Kích hoạt CORS (phải đặt trước Authentication/Authorization)
app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

// Map controllers to enable API endpoints
app.MapControllers();

app.Run();