using WebBrowser.Services.Interfaces;
using Serilog;
using WebBrowser.Services.Implements;
using WebBrowser.Services.ApiServices;

using CommonLib.Helpers;
using CommonLib.Implementations;
using CommonLib.Interfaces;
using CommonLib.Handles;
using WebBrowser.Services.Implementations;

var builder = WebApplication.CreateBuilder(args);
// Tích hợp Serilog (đọc cấu hình từ appsettings.json)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithThreadId()
    .Enrich.WithMachineName()
    .CreateLogger();
builder.Services.AddHttpContextAccessor();

builder.Host.UseSerilog();

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<IHttpService, HttpService>();
builder.Services.AddScoped<IDepartmentMvcService, DepartmentMvcService>();
builder.Services.AddScoped<IDataConvertHelper, DataConvertHelper>();
builder.Services.AddScoped<ISerilogProvider, SerilogProvider>();
builder.Services.AddScoped<IErrorHandler, ErrorHandler>();
builder.Services.AddScoped<IEmpService,EmpService>();
builder.Services.AddScoped<IJob1Service, Job1Service>();
builder.Services.AddSession();
Log.Information("Ứng dụng đang khởi động...");
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

app.Run();