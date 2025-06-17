    using WebBrowser.Services;
    using WebBrowser.Services.Interfaces;
    using Serilog;
using WebBrowser.Services.Implements;
var builder = WebApplication.CreateBuilder(args);
// Tích hợp Serilog (đọc cấu hình từ appsettings.json)
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .Enrich.WithThreadId()
        .Enrich.WithMachineName()
        .CreateLogger();

    builder.Host.UseSerilog();
    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddHttpClient<IDepartmentService, DepartmentService>();
    builder.Services.AddHttpClient<IJobService, JobService>();
    builder.Services.AddHttpClient<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ILoginService, LoginService>();
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
