using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using HEI.Support.Service.Implementation;
using HEI.Support.Service.Interface;
using Microsoft.AspNetCore.Identity;
using HEI.Support.Infrastructure.Persistence;
using HEI.Support.Domain.Entities;
using HEI.Support.Common.Models;
using HEI.Support.Infrastructure.Persistence.Repository.Interface;
using HEI.Support.Infrastructure.Persistence.Repository.Implementation;
using HEI.Support.Infrastructure.SeedData;

var builder = WebApplication.CreateBuilder(args);

// Configure services
builder.Services.AddControllersWithViews();

// Database connection setup
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Session and caching
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// Identity setup
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(365 * 10);
    options.Lockout.MaxFailedAccessAttempts = 10;

    options.Tokens.EmailConfirmationTokenProvider = "Default";
    options.Tokens.PasswordResetTokenProvider = "Default";
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Cookie configuration
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// SMTP configuration
builder.Services.Configure<SMTPConfig>(builder.Configuration.GetSection("SMTPConfig"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<SMTPConfig>>().Value);
builder.Services.AddSingleton<SmtpClient>(sp =>
{
    var smtpSettings = sp.GetRequiredService<SMTPConfig>();
    return new SmtpClient
    {
        Host = smtpSettings.SmtpHost,
        Port = smtpSettings.SmtpPort,
        EnableSsl = smtpSettings.SmtpSsl,
        DeliveryMethod = SmtpDeliveryMethod.Network,
        UseDefaultCredentials = false,
        Credentials = new NetworkCredential(smtpSettings.SmtpUsername, smtpSettings.SmtpPassword)
    };
});

// Dependency injection for repositories and services
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<IAttachmentFileRepository, AttachmentFileRepository>();
builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ITicketService, TicketService>();

var app = builder.Build();

// Seed roles and users
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    await SeedRoleAndUser.Initialize(services, userManager);
}

// Middleware configuration
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseSession(); // Ensure session middleware is included

// Route configuration
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Admin}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
