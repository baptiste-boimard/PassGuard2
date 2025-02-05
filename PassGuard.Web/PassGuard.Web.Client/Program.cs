using System.Text;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.IdentityModel.Tokens;
using MudBlazor.Services;
using PassGuard.Web.Client.Service;

// using PassGuard.Api.Service;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices();

builder.Services.AddBlazoredLocalStorage();

// builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//     .AddJwtBearer(options =>
//     {
//         options.RequireHttpsMetadata = false;  // 🔥 Désactiver pour le test en local
//         options.SaveToken = true;
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = false,
//             ValidateAudience = false,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("VotreCléSecrèteTrèsLongue"))
//         };
//     });

// Pour le côté client, on utilise AddAuthorizationCore()
// builder.Services.AddAuthorizationCore();
// builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<AuthStateProvider>());
// builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
// builder.Services.AddScoped<AuthStateProvider>(); // Ajout direct du service


// Enregistrer HttpClient pour le client WebAssembly
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:7012") // Remplacez par votre API si besoin
});

await builder.Build().RunAsync();