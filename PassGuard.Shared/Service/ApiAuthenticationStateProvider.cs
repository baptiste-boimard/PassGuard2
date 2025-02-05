// using System.IdentityModel.Tokens.Jwt;
// using System.Security.Claims;
// using Blazored.LocalStorage;
// using Microsoft.AspNetCore.Components.Authorization;
//
// namespace PassGuard.Api.Service;
//
// public class ApiAuthenticationStateProvider : AuthenticationStateProvider
// {
//     private readonly ILocalStorageService _localStorage;
//
//     public ApiAuthenticationStateProvider(ILocalStorageService localStorage)
//     {
//         _localStorage = localStorage;
//     }
//
//     public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//     {
//         // Récupération du token depuis le local storage
//         var token = await _localStorage.GetItemAsync<string>("token");
//         ClaimsIdentity identity;
//
//         if (string.IsNullOrWhiteSpace(token))
//         {
//             // Aucun token : utilisateur non authentifié
//             identity = new ClaimsIdentity();
//         }
//         else
//         {
//             // Extraire les claims du token
//             var claims = ParseClaimsFromJwt(token);
//             identity = new ClaimsIdentity(claims, "jwt");
//         }
//
//         var user = new ClaimsPrincipal(identity);
//         return new AuthenticationState(user);
//     }
//     
//     // Méthode pour extraire les claims d'un JWT
//     private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
//     {
//         var handler = new JwtSecurityTokenHandler();
//         var token = handler.ReadJwtToken(jwt);
//         return token.Claims;
//     }
//
//     // Notifier la connexion d'un utilisateur
//     public void NotifyUserAuthentication(string token)
//     {
//         var claims = ParseClaimsFromJwt(token);
//         var identity = new ClaimsIdentity(claims, "jwt");
//         var user = new ClaimsPrincipal(identity);
//         NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
//     }
//
//     // Notifier la déconnexion d'un utilisateur
//     public void NotifyUserLogout()
//     {
//         var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
//         NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymousUser)));
//     }
// }