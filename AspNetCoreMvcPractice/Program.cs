using AspNetCoreMvcPractice.Core.Interfaces;
using AspNetCoreMvcPractice.Core.Services;
using AspNetCoreMvcPractice.Infrastructure.Data;
using AspNetCoreMvcPractice.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DI container configuration
// Register the NorthwindContext service
var connectionString = builder.Configuration.GetConnectionString("NorthwindConnection");
builder.Services.AddDbContext<NorthwindContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddHttpClient();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
