using Microsoft.EntityFrameworkCore;
using Quartz;
using QuartzAPI;
using QuartzAPI.Middleware;
using QuartzAPI.Services;
using QuartzAPI.Utilities;
using QuartzData;
using QuartzData.Repositories;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var corsPolicyName = "AllowBlazorUI";

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSerilog(builder.Configuration);

builder.Services.AddDataContexts(builder.Configuration);

builder.Services.AddServices();
builder.Services.AddHttpServices();
builder.Services.AddRepositories();

builder.Services.AddCors(builder.Configuration, corsPolicyName);

builder.Services.AddQuartz();

MapsterConfig.Configure();
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(corsPolicyName);

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
