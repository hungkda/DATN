using Microsoft.EntityFrameworkCore;
using DATN.Models;
using DATN.Services;
using DATN.ViewModels;
using OfficeOpenXml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Cau hinh ket noi
var configuration = builder.Configuration;
var connectionString = builder.Configuration.GetConnectionString("AppConnection");
builder.Services.AddDbContext<DATNDbContext>(x => x.UseSqlServer(connectionString));

//C?u hình s? d?ng session
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".DATN.Session";
});

// Thiết lập LicenseContext cho EPPlus
ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

//add email config
builder.Services.AddControllersWithViews();

// Đăng ký EmailConfiguration
var emailConfig = configuration.GetSection("EmailConfiguration")
    .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

// Đăng ký IEmailServices
builder.Services.AddTransient<IEmailServices, EmailServices>();

//??ng kí d?ch v? cho HttpContextAccessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

//S? d?ng session ?ã khai báo ? trên
app.UseSession();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();
