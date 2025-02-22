using Blazored.LocalStorage;
using MudBlazor.Services;
using PassGuard.Shared.State;
using PassGuard.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMudServices();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddScoped<PasswordState>();
builder.Services.AddScoped<UserState>();

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost:7012") });

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

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(PassGuard.Web.Client._Imports).Assembly);

await app.RunAsync();