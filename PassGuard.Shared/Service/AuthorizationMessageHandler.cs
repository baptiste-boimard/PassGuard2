// using Blazored.LocalStorage;
//
// namespace PassGuard.Api.Service;
//
// public class AuthorizationMessageHandler : DelegatingHandler
// {
//     private readonly ILocalStorageService _localStorage;
//
//     public AuthorizationMessageHandler(ILocalStorageService localStorage)
//     {
//         _localStorage = localStorage;
//     }
//
//     protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//     {
//         var token = await _localStorage.GetItemAsync<string>("token");
//         if (!string.IsNullOrWhiteSpace(token))
//         {
//             Console.WriteLine("Ajout du token dans l'en-tête Authorization.");
//             request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
//         }
//         else
//         {
//             Console.WriteLine("Aucun token trouvé dans le local storage.");
//         }
//         return await base.SendAsync(request, cancellationToken);
//     }
// }