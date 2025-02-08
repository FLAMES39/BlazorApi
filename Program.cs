using BlazorApi.Data;
using BlazorApi.Interfaces;
using BlazorApi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MariaDbConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MariaDbConnection"))));

//var connectionString = builder.Configuration.GetConnectionString("MariaDbConnection");

//builder.Services.AddDbContext<DataContext>(options =>
   // options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

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
