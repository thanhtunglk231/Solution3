using CommonLib.Handles;
using CommonLib.Helpers;
using CommonLib.Implementations;
using CommonLib.Interfaces;
using DataServiceLib.Implementations;
using DataServiceLib.Implementations1;
using DataServiceLib.Interfaces;
using DataServiceLib.Interfaces1;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using DataServiceLib.unuse.Implementations;
using DataServiceLib.unuse.Interfaces.unuse;
using CommonLib.unuse;

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

// ------------------ Swagger ------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------ DI Services ------------------
builder.Services.AddScoped<ICEmpDataProvider, CEmpDataProvider>();
builder.Services.AddScoped<ICAccountDataProvider,CAccountDataProvider>();
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

// ------------------ Build & Run App ------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Log.Information("Phía backend...");

app.Run();
