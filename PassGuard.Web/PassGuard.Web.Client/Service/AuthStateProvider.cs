using System.Security.Claims;
using System.Text.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using PassGuard.Shared.Models;

namespace PassGuard.Web.Client.Service;

// public class AuthStateProvider : AuthenticationStateProvider
// {
//     private readonly ILocalStorageService _localStorageService;
//     private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());
//
//     public AuthStateProvider (ILocalStorageService  localStorageService)
//     {
//         _localStorageService = localStorageService;
//     }    
//     
//     public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//     {
//         var token = await _localStorageService.GetItemAsync<JwtToken>("token");
//
//         return new AuthenticationState(_currentUser);
//     }
//     public async Task LoadUserFromStorage()
//     {
//         var token = await _localStorageService.GetItemAsync<JwtToken>("token");
//
//         if (!string.IsNullOrEmpty(token.Token))
//         {
//             var claims = ParseClaimsFromJwt(token.Token);
//             var identity = new ClaimsIdentity(claims, "jwt");
//             _currentUser = new ClaimsPrincipal(identity);
//         }
//         else
//         {
//             _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
//         }
//
//         NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
//     }
//     
//     // public async Task MarkUserAsAuthenticated (JwtToken token)
//     // {
//     //     await _localStorageService.SetItemAsync("token", token);
//     //     
//     //     var claims = ParseClaimsFromJwt(token.Token);
//     //     var identity = new ClaimsIdentity(claims, "jwt");
//     //     var user = new ClaimsPrincipal(identity);
//     //     
//     //     NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
//     // }
//
//     // public void MarkUserAsLoggedOut()
//     // {
//     //     var user = new ClaimsPrincipal(new ClaimsIdentity());
//     //     NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
//     // }
//
//     private IEnumerable<Claim> ParseClaimsFromJwt(string token)
//     {
//         var payload = token.Split('.')[1];
//         var jsonBytes = Convert.FromBase64String(payload);
//         var keyValuePairs = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
//         
//         return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
//     }
// }

public class AuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorageService;
    private ClaimsPrincipal _currentUser = new(new ClaimsIdentity());

    private bool _isInitialized = false;

    public AuthStateProvider(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        
        var token = await _localStorageService.GetItemAsync<string>("token");

        if (!string.IsNullOrEmpty(token))
        {
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            _currentUser = new ClaimsPrincipal(identity);
        }
        else
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        }

        return new AuthenticationState(_currentUser);
    }

    public async Task LoadUserFromStorage()
    {
        var token = await _localStorageService.GetItemAsync<string>("token");

        if (!string.IsNullOrEmpty(token))
        {
            var claims = ParseClaimsFromJwt(token);
            var identity = new ClaimsIdentity(claims, "jwt");
            _currentUser = new ClaimsPrincipal(identity);
        }
        else
        {
            _currentUser = new ClaimsPrincipal(new ClaimsIdentity());
        }

        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_currentUser)));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string token)
    {
        var payload = token.Split('.')[1];
        var jsonBytes = Convert.FromBase64String(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);
        
        return keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()));
    }
}