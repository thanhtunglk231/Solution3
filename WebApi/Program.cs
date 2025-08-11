using CommonLib.Handles;
using CommonLib.Helpers;
using CommonLib.Implementations;
using CommonLib.Interfaces;
using CommonLib.unuse;
using CoreLib.Dtos;

using DataServiceLib.Hubs;
using DataServiceLib.Implementations;
using DataServiceLib.Implementations1;
using DataServiceLib.Interfaces;
using DataServiceLib.Interfaces1;
using DataServiceLib.unuse.Implementations;
using DataServiceLib.unuse.Interfaces.unuse;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------ Logging ------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog();

// ------------------ JWT Auth ------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]))
    };
});

// ------------------ Controller + Newtonsoft.Json ------------------
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.None;
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// ------------------ SMTP ------------------
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

// ------------------ Google Drive Service ------------------
builder.Services.AddSingleton<IGoogleDriveService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var jsonPath = Path.Combine(env.ContentRootPath, "optimal-buffer-468407-c1-50cb38ab813b.json"); // File đặt ở thư mục gốc WebApi

    if (!File.Exists(jsonPath))
    {
        Log.Error("❌ Không tìm thấy file Google Service Account tại: {Path}", jsonPath);
        throw new FileNotFoundException("Không tìm thấy file Google Service Account.", jsonPath);
    }

    Log.Information("✅ Đang khởi tạo GoogleDriveService với file: {Path}", jsonPath);
    return new GoogleDriveService(jsonPath);
});


builder.Services.AddSingleton<ISupabaseService, SupabaseService>();
// ------------------ Redis ------------------
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(config);
});
builder.Services.AddScoped<IRedisService, RedisService>();

// ------------------ Swagger ------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------ DI Services ------------------

builder.Services.AddScoped<IOtpService, OtpService>();
builder.Services.AddScoped<ICEmpDataProvider, CEmpDataProvider>();
builder.Services.AddScoped<ICAccountDataProvider, CAccountDataProvider>();
builder.Services.AddScoped<ICBaseDataProvider, CBaseDataProvider>();
builder.Services.AddScoped<ICBaseDataProvider1, CBaseDataProvider1>();
builder.Services.AddScoped<IDataConvertHelper, DataConvertHelper>();
builder.Services.AddScoped<ICDepartmentDataProvider, CDepartmentDataProvider>();
builder.Services.AddScoped<ICDepartmentDataProvider1, CDepartmentDataProvider1>();
builder.Services.AddScoped<IJobLogicHandler, JobLogicHandler>();
builder.Services.AddScoped<ICJobDataProvider, CJobDataProvider>();
builder.Services.AddScoped<ICJob1DataProvider, CJob1DataProvider>();
builder.Services.AddScoped<ICEmployeeDataProvider, CEmployeeDataProvider>();
builder.Services.AddScoped<ICLoginProvider, CLoginProvider>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IErrorHandler, ErrorHandler>();
builder.Services.AddScoped<DataTableHelper>();
builder.Services.AddSingleton<ISerilogProvider, SerilogProvider>();
builder.Services.AddScoped<ICChat, CChat>();
builder.Services.AddSignalR();

// ------------------ Build & Run App ------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<ChatHub>("/chathub");
});

Log.Information("🚀 Backend đã khởi động...");
app.Run();
