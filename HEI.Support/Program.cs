using HEI.Support.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using HEI.Support.Areas.Admin.Models;
using HEI.Support.Models;
using HEI.Support.Service.Implementation;
using HEI.Support.Service.Interface;
using HEI.Support.Data.Entities;
using Microsoft.AspNetCore.Identity;
using HEI.Support.Data.SeedData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<SMTPConfig>(builder.Configuration.GetSection("SMTPConfig"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<SMTPConfig>>().Value);
builder.Services.AddSingleton<SmtpClient>(sp =>
{
    var smtpSettings = sp.GetRequiredService<SMTPConfig>();
    var smtpClient = new SmtpClient
    {
        Host = smtpSettings.SmtpHost,
        Port = smtpSettings.SmtpPort,
        EnableSsl = smtpSettings.SmtpSsl,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(smtpSettings.SmtpUsername, smtpSettings.SmtpPassword)
    };
    return smtpClient;
});

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IAccountService, AccountService>();
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
	await SeedRoleAndUser.Initialize(services, userManager);  // Ensure this is awaited
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
