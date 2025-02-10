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
        options.LicenseKey = "eyJhdXRoIjoiREVNTyIsImV4cCI6IjIwMjUtMDMtMTBUMDA6MDA6MDAiLCJpYXQiOiIyMDI1LTAyLTEwVDE0OjIyOjMwIiwib3JnIjoiREVNTyIsImF1ZCI6Nn0=.RRSEaP/HzG8axYuHRc/hSfpxWbU+hW71xIL+4Kv/oHxe9YFA1FIYT+IaWakzp6NIxudr/cd2L0tcVNuwtP4+5452OivubMWvbNvX0xRDXPFlKwkzn97Y7EB9AfBdQqPFi/Uok6ZxBg7eOJFceCywAwrEQWBqfpX+DX02NGPuWme5V6omwocqiHCEuUzkOIB7i/JRRdQQtXtXuWSDGBpGSC0NBySrH3R4tbAY0t/z8thHau8dKNKXvgUoeSlMYYR0OgZkSFXIEkfbabEDj7So9FdD4UoJDEe4neOoZXXPRJVARucYAnqin5X1bGzpgOVNEbxRnYvA0OKvgurJsLJc6ivQV8oRhXX92RX3Ptas1Vd+e9DK/aBE0Q0oMHsGm0VIc1eVUgC+npihUU89aEHEY935APJGtw9bk1hlPtxgWGRj8zSSUwyY6W+Td0vgy16JZbHeOTqNcM6t6/l4T2w7Texi5w37R4qdIkaRi5p5EZ5uMjhhb1EuoKYfVvhXEBl91AT+EEZoMdzgK0ng4bpONFzwsuR/6ub0m/ijKBmsCp2y7UH5ckb9AE2osUK6BE8Ia/nHCuclU/0wY0Xz4MPkAQBN2WwmmFX0naZQ768+aoACk5JWZoPTJruQ/XsKU6ilmDllk6g/ByShZecPBwipIBJi0csUHUJKQlv4r8ezVgw=";
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