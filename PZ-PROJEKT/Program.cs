using Microsoft.EntityFrameworkCore;
using PZ_PROJEKT.Data;
using PZ_PROJEKT.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PZ_PROJEKTContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("PZ_PROJEKTContext")));

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

builder.Services.AddAuthentication("CookieAuthentication")
    .AddCookie("CookieAuthentication", options =>
    {
        options.Cookie.Name = "UserLoginCookie";
        options.LoginPath = "/Auth";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HTTP only
    options.Cookie.IsEssential = true; // Make the session cookie essential
});

var app = builder.Build();

// db seeding
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;

	SeedData.Initialize(services);
}

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
app.UseSession();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();