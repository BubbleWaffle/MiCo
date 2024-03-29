﻿using MiCo.Data;
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
builder.Services.AddScoped<IHomeService, HomeService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IJusticeService, JusticeService>();
builder.Services.AddScoped<IThreadService, ThreadService>();

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