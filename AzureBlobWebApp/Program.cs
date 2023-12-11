using Serilog;
using System.Text;
using AzureBlobWebApp.BusinessLayer.DTOs;
using AzureBlobWebApp.BusinessLayer.Interfaces;
using AzureBlobWebApp.BusinessLayer.Services;
using AzureBlobWebApp.DataLayer.Models;
using AzureBlobWebApp.DataLayer.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AzureBlobWebAppDbContext>(options => options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("Database")));

builder.Services.AddScoped<ILoginService, LoginService>();
builder.Services.AddScoped<IAzureBlobService, AzureBlobService>();
builder.Services.AddScoped<IDataRepository, DataRepository>();


var _jwtsetting = builder.Configuration.GetSection("JWTSetting");
builder.Services.Configure<JWTSetting>(_jwtsetting);

var _azureblobsetting = builder.Configuration.GetSection("AzureBlobSetting");
builder.Services.Configure<AzureBlobSetting>(_azureblobsetting);

var authkey = builder.Configuration.GetValue<string>("JWTSetting:SecurityKey");

builder.Services.AddAuthentication(item =>
{
    item.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    item.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(item =>
{
    item.RequireHttpsMetadata = true;
    item.SaveToken = true;
    item.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authkey)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console()  //To write in Console 
   .ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("api", new OpenApiInfo()
    {
        Description = "Azure Blob operations API with crud operations",
        Title = "User",
        Version = "1"
    });
});

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("api/swagger.json", "User"));

app.UseCors(builder =>
{
    builder
    .WithOrigins("http://localhost:44417/")
    .AllowAnyMethod()
    .AllowAnyHeader();
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();
