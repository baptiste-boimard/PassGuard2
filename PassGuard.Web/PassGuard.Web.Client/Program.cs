using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PassGuard.Shared.State;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<PasswordState>();
builder.Services.AddScoped<UserState>();

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost:7012") });

await builder.Build().RunAsync();