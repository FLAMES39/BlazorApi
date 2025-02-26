//using BlazorApi.Data;
//using BlazorApi.DataService;
//using BlazorApi.Interfaces;
//using BlazorApi.Services;
//using Microsoft.AspNetCore.Cors.Infrastructure;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.FileProviders;

//var builder = WebApplication.CreateBuilder(args);
//builder.Services.AddControllers();
//builder.Services.AddAuthorization();
//builder.Services.AddSwaggerGen();
//builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<JobService>();
//builder.Services.AddScoped<IJobs, JobService>();
//builder.Services.AddScoped<IAuthService, AuthService>();
//builder.Services.AddScoped<ApplicationService>();
//builder.Services.AddScoped<TemporaryCredentialsService>();
//builder.Services.AddScoped<ITempCredemtials, TemporaryCredentialsService>();


////builder.Services.AddScoped<IApplications, ApplicationService>();
//builder.Services.AddScoped<IApplications, ApplicationService>();


//// Add services to the container.
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(policy =>
//    {
//        policy.AllowAnyOrigin()
//              .AllowAnyHeader()
//              .AllowAnyMethod();
//    });
//});

//builder.Services.AddDbContext<DataContext>(options =>
//    options.UseMySql(
//        builder.Configuration.GetConnectionString("MariaDbConnection"),
//        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MariaDbConnection"))));
////var connectionString = "Server=localhost;Database=job_recruitment_db;User=root;Password=SUNPRO100#;Port=3306";
////builder.Services.AddSingleton(new DatabaseService(connectionString));

////var connectionString = builder.Configuration.GetConnectionString("MariaDbConnection");

////builder.Services.AddDbContext<DataContext>(options =>
//// options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
//var connectionString = "Server=localhost;Database=job_recruitment_db;User=root;Password=SUNPRO100#;Port=3306";

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//builder.Services.AddSingleton<IDataService>(sp =>
//{
//    var logger = sp.GetRequiredService<ILogger<DataService>>();
//    return new DataService(connectionString, logger);
//});
//app.UseHttpsRedirection();
//app.UseCors(builder =>
//    builder.AllowAnyOrigin()
//           .AllowAnyMethod()
//           .AllowAnyHeader());
//app.UseAuthorization();


//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
//    RequestPath = "/Uploads"
//});


//app.MapControllers();

//app.UseStaticFiles();




//app.Run();


using BlazorApi.Data;
using BlazorApi.DataService;
using BlazorApi.Interfaces;
using BlazorApi.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

// ?? Register Controllers, Swagger, and CORS
builder.Services.AddControllers();
builder.Services.AddAuthorization();
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

// ?? Register Services (BEFORE `builder.Build()`)
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<JobService>();
builder.Services.AddScoped<IJobs, JobService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ApplicationService>();
builder.Services.AddScoped<TemporaryCredentialsService>();
builder.Services.AddScoped<ITempCredemtials, TemporaryCredentialsService>();
builder.Services.AddScoped<IApplications, ApplicationService>();

// ?? Register Database Context (EF Core)
builder.Services.AddDbContext<DataContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MariaDbConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MariaDbConnection"))));

// ?? Register Dapper-Based Service (Singleton)
var connectionString = "Server=localhost;Database=job_recruitment_db;User=root;Password=SUNPRO100#;Port=3306";
builder.Services.AddSingleton<IDataService>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<DataService>>();
    return new DataService(connectionString, logger);
});

// ?? Build the application AFTER service registrations
var app = builder.Build();

// ?? Configure Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ?? CORS should be BEFORE `UseAuthorization()`
app.UseCors(builder =>
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseAuthorization();

// ?? Serve Static Files (Only Once)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Uploads"
});

// ?? Map Controllers
app.MapControllers();

// ?? Run Application
app.Run();

