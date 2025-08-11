using CommonLib.Handles;
using CommonLib.Helpers;
using CommonLib.Implementations;
using CommonLib.Interfaces;

using DataServiceLib.Hubs;
using DataServiceLib.Implementations1;
using DataServiceLib.Interfaces1;
using Serilog;
using StackExchange.Redis;
using WebBrowser.Services.ApiServices;
using WebBrowser.Services.Implementations;
using WebBrowser.Services.Implements;
using WebBrowser.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// ------------------- Serilog Logging -------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithMachineName()
    .CreateLogger();

builder.Host.UseSerilog();

// ------------------- Add Services -------------------
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IDepartmentMvcService, DepartmentMvcService>();
builder.Services.AddScoped<IDataConvertHelper, DataConvertHelper>();
builder.Services.AddScoped<ISerilogProvider, SerilogProvider>();
builder.Services.AddScoped<IErrorHandler, ErrorHandler>();
builder.Services.AddScoped<IEmpService, EmpService>();
builder.Services.AddScoped<IJob1Service, Job1Service>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<ICBaseDataProvider1, CBaseDataProvider1>();
builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(config);
});
builder.Services.AddScoped<IRedisService, RedisService>();

// ------------------- Chat (SignalR) -------------------
builder.Services.AddScoped<ICChat, CChat>();
builder.Services.AddSignalR();

// ------------------- Session -------------------
builder.Services.AddSession();

Log.Information("Ứng dụng đang khởi động...");
var app = builder.Build();

// ------------------- Middleware Pipeline -------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chathub");

app.Run();
