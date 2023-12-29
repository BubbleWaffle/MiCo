using MiCo.Data;
using MiCo.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

/* Connect database */
builder.Services.AddDbContext<MiCoDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);

/* Configure file transfer (60MB max) */
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 60000000;
});

/* Add services */
builder.Services.AddScoped<RegistrationService>();
builder.Services.AddScoped<LoginService>();
builder.Services.AddScoped<ProfileContentService>();
builder.Services.AddScoped<ProfileEditService>();
builder.Services.AddScoped<ProfileReportService>();
builder.Services.AddScoped<ProfileDeleteService>();
builder.Services.AddScoped<BanService>();
builder.Services.AddScoped<UnbanService>();
builder.Services.AddScoped<JusticeContentService>();
builder.Services.AddScoped<SaveService>();
builder.Services.AddScoped<ThreadCreateService>();

/* Add hosted services */
builder.Services.AddHostedService<AutoUnbanService>();

/* Configure session */
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.MaxValue;
    options.Cookie.Name = "MiCoCookie";
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();