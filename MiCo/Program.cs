using MiCo.Data;
using MiCo.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MiCoDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);

builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<ProfileService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.MaxValue; // Ustaw IdleTimeout na nieskończoność
    options.Cookie.Name = "MiCoCookie"; // Dostosuj nazwę ciasteczka sesji
});

// Dodaj HttpContextAccessor
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(); // Dodaj schemat uwierzytelniania opartego na ciasteczkach (cookies)

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

// Dodaj obsługę sesji między UseRouting() a UseAuthorization()
app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
