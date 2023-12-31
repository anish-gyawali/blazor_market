using Blazor_Market.Provider;
using Blazor_Market.Services.AuthenticationService;
using Blazor_Market.Services.ProductService;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthenticationCore();
builder.Services.AddScoped<HttpClient>(s =>
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration.GetSection("ApiBaseUrl").Value!)
    };
    return httpClient;
});
builder.Services.AddHttpClient();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IAuthenticationService,AuthenticationService>();

builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(p=>
                p.GetRequiredService<ApiAuthenticationStateProvider>());

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

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
