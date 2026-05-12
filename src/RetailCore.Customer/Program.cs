using Microsoft.AspNetCore.Authentication.Cookies;
using Refit;
using RetailCore.Customer.Services;
using RetailCore.Customer.Services.Api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Api
var apiOptions = builder.Configuration
    .GetSection("Api")
    .Get<ApiOptions>()
    ?? throw new InvalidOperationException("Api config missing");

builder.Services
    .AddRefitClient<IAuthApi>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri(apiOptions.BaseUrl))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services
    .AddRefitClient<IProductApi>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri(apiOptions.BaseUrl))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services
    .AddRefitClient<ICategoryApi>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri(apiOptions.BaseUrl))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services
    .AddRefitClient<IBrandApi>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri(apiOptions.BaseUrl))
    .AddHttpMessageHandler<AuthTokenHandler>();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.Cookie.Name = "RetailCore.Customer.Auth";
        options.ExpireTimeSpan = TimeSpan.FromDays(7);
        options.SlidingExpiration = true;
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddTransient<AuthTokenHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
