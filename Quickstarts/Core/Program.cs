using Microsoft.Extensions.DependencyInjection.Extensions;
using Rsk.AspNetCore.Fido.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.TryAddSingleton<IDateTimeProvider, DateTimeProvider>();
builder.Services.AddAntiforgery(o => o.HeaderName = "XSRF-TOKEN");

builder.Services.AddFido(options =>
    {

        options.Licensee = "DEMO";
        options.LicenseKey = "Get your license key from https://www.identityserver.com/products/fido2-for-aspnet";
    })
    .AddInMemoryKeyStore();

builder.Services.AddAuthentication("cookie")
    .AddCookie("cookie", options => { options.LoginPath = "/Login/Index"; });
var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();