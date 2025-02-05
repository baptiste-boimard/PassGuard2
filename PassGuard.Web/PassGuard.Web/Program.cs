using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using PassGuard.Web.Client.Service;
// using PassGuard.Api.Service;
using PassGuard.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

// Ajout du service d'authorisation

builder.Services.AddBlazoredLocalStorage();

// Enregistrer le service d'authentification avec le schéma JWT
// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = builder.Configuration["Jwt:issuer"],
//             ValidAudience = builder.Configuration["Jwt:audience"],
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:jwtSecret"]))
//         };
//     });

// builder.Services.AddAuthorizationCore();
// builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
// builder.Services.AddScoped<AuthStateProvider>();

// Enregistrer HttpClient pour le client WebAssembly
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7012") // Remplacez par votre API si besoin
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

// app.UseAuthentication();
// app.UseAuthorization();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(PassGuard.Web.Client._Imports).Assembly);
    // .AddAdditionalAssemblies(typeof(PassGuard.Web.Components.Routes).Assembly);



await app.RunAsync();