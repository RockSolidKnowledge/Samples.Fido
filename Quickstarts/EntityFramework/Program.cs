using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Rsk.AspNetCore.Fido.Services;

// Create the builder
var builder = WebApplication.CreateBuilder(args);

// Pull Fido configuration from appsettings.json
var fidoConfig = builder.Configuration.GetSection("Fido");
string licensee = fidoConfig["Licensee"] ?? "DEMO";
string licenseKey = fidoConfig["LicenseKey"] ?? "Get license key from https://www.identityserver.com/products/fido2-for-aspnet";

// Get connection string from configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

// Add services
builder.Services.AddRazorPages();
builder.Services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddAntiforgery(options => options.HeaderName = "XSRF-TOKEN");

// Configure FIDO using values from appsettings
builder.Services.AddFido(options =>
{
    options.Licensee = licensee;
    options.LicenseKey = licenseKey;
})
.AddEntityFrameworkStore(options => 
    options.UseSqlite(connectionString, sql => sql.MigrationsAssembly(migrationsAssembly)));

// Configure Cookie authentication
builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie", options =>
    {
        options.LoginPath = "/Login/Index";
    });

// Build the application
var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Make sure to use Authentication before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
