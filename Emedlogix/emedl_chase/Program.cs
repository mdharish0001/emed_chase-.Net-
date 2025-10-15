using emedl_chase;
using emedl_chase.DbModel;
using emedl_chase.Option;
using emedl_chase.Repository;
using emedl_chase.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<client_uploadsService>();
builder.Services.AddScoped<charge_captureService>();
builder.Services.AddScoped<OrganizationService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


builder.Services.Configure<ApplicationConfig>(
    builder.Configuration.GetSection("ApplicationConfig"));

builder.Services.AddDbContext<ChaseDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DBContext")));

string connectionString = builder.Configuration.GetConnectionString("DBContext");

//StaticDataService.InitializeData(connectionString, environment);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();


app.UseStaticFiles();

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

