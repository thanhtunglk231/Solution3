using CommonLib.Implementations;

using DataServiceLib.Implementations;
using DataServiceLib.Interfaces;
using WebApi.Handles;
using WebApi.Service.Implement;
using WebApi.Service.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<ICBaseDataProvider, CBaseDataProvider>();
builder.Services.AddScoped<DataTableHelper>();
builder.Services.AddScoped<IJobService,JobService>();
builder.Services.AddScoped<IJobLogicHandler, JobLogicHandler>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
