using AzubiLog.Components;
using AzubiLog.Data;
using AzubiLog.Models;
using AzubiLog.Services.Dashboard;
using AzubiLog.Services.Identity;
using AzubiLog.Services.ReportEntries;
using AzubiLog.Services.Timetable;
using AzubiLog.Services.Todos;
using AzubiLog.Services.Trainer;
using AzubiLog.Services.WeeklyReports;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Services ---
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddHttpContextAccessor();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedEmail = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddClaimsPrincipalFactory<AppUserClaimsPrincipalFactory>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/account/login";
    options.LogoutPath = "/account/logout";
    options.AccessDeniedPath = "/account/access-denied";
});

// Application services
builder.Services.AddScoped<ApplicationDataInitializer>();
builder.Services.AddScoped<AccountFlowService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IReportEntryService, ReportEntryService>();
builder.Services.AddScoped<ITodoService, TodoService>();
builder.Services.AddScoped<ITimetableService, TimetableService>();
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IWeeklyReportService, WeeklyReportService>();

// Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireKlassensprecher",
        policy => policy.RequireRole(AppRole.Klassensprecher));
    options.AddPolicy("RequireAusbilder",
        policy => policy.RequireRole(AppRole.Ausbilder));
});

var app = builder.Build();

// --- Database initialization ---
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDataInitializer>();
    await initializer.InitializeAsync();
}

// --- Middleware ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapAccountEndpoints();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
