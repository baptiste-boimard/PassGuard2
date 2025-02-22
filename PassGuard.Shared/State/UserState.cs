using System.Net.Http.Json;


namespace PassGuard.Shared.State;

public class UserState
{
    private readonly HttpClient _httpClient;
    public UserState(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public event Action? OnChange;

    public string? Email { get; set; }


    public async Task<string> GetEmail(string token)
    {
        var content = JsonContent.Create(token);
        
        var response = await _httpClient.PostAsync("https://localhost:7012/api/auth/getemail", content);

        if (response.IsSuccessStatusCode)
        {
            Email = await response.Content.ReadAsStringAsync();
            OnChange?.Invoke();
        }

        return null;
    }
}
