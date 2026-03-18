using CodeQLTour.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<TourContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TourContext")));

// THÊM DÒNG NÀY
builder.Services.AddSession();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();

app.UseRouting();

// THÊM DÒNG NÀY
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
   pattern: "{controller=Tour}/{action=Index}/{id?}");

app.Run();