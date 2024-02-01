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
        options.LicenseKey = "eyJTb2xkRm9yIjowLjAsIktleVByZXNldCI6NiwiU2F2ZUtleSI6ZmFsc2UsIkxlZ2FjeUtleSI6ZmFsc2UsIlJlbmV3YWxTZW50VGltZSI6IjAwMDEtMDEtMDFUMDA6MDA6MDAiLCJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjQtMDctMDhUMDA6MDA6MDAiLCJpYXQiOiIyMDI0LTAxLTA4VDE2OjEzOjEzIiwib3JnIjoiREVNTyIsImF1ZCI6Nn0=.Tb5ayoJdcb7uoMU9F9Jr58ldcanmVwg/Krn1e9QTUt2PhcKqS785l0SZ5oKKikPdSl5eUZ4TLmpWYbzsCeRavnR45GnvAHLIKu5wYhOkqaHtG3nc5+VJ4v1nUlQsz7eCxdT33I8w6zlgNqiDDE2M7TAWBTkdvxx8l/xQRa5LD4gf5wNij0TE9XYmLIPylopbcncKPN3lydfIPLXmXNzn9/V28LbNQRcgE0dU3T2j3v6O+Dlu1l6SLzf4iENkv+87QveAgU9Od/Sf9PyewNs8qVBsQtYFuovaa9HIyxxS0+TnEVhYqhd9kDQXUVZGGlg3+xJ3CO5Z6iimb3tqoxexGjsg+RBMcERegWMvC333oY6dma4sFIZnBbmQ4UNaNgxzZYb77IJ88h8SKNVgbas1rCpbUJhlenvUv0rkO7hldB5WJVHNUJntAIi4rEvdADMMoILrK7t8NVKKa+foiZiJwfEtqQkMRkkDc8it3PThIlX3F7RI6hd7PCK6AsVpgCS6f/jAUv+S5GTewvTXg8wCooXZpEmKR4qjwRO1RAWTjYS2yR2EGJdrMXsAMj32wzstKK5xiYAtcaPQeNz8cWTfFF9jaYcVoxzpCZAtnzDqVl56IAcDjvrOX94pJbWhRoJ7d54tMOKEyr+jt8Yrfx852wgx2J/uY13VO6/wkHUYhBE=";
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