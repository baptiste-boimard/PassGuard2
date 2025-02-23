using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using PassGuard.Shared.Models;


namespace PassGuard.Shared.State;

public class PasswordState
{
    private readonly HttpClient _httpClient;

    public PasswordState(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
    public ObjectPassword[] PasswordArray { get; private set; } = Array.Empty<ObjectPassword>();

    public event Action? OnChange;

    public void AddPassword(ObjectPassword objectPassword)
    {
        var list = PasswordArray.ToList();
        list.Add(objectPassword);
        PasswordArray = list.ToArray();
        OnChange?.Invoke();
    }

    public void SetPasswordArray(ObjectPassword[] passwordArray)
    {
        PasswordArray = passwordArray;
        OnChange?.Invoke();
    }

    public async Task<ObjectPassword[]> GetPasswordArray(string? token)
    {
        var content = JsonContent.Create(token); 
        
        var response = await _httpClient.PostAsync("https://localhost:7012/api/password/getpassword", content);
        
        if (response.IsSuccessStatusCode)
        {
            ObjectPassword[] passwordArray = (await response.Content.ReadFromJsonAsync<ObjectPassword[]>())
                .OrderBy(p => p.Category)
                .ToArray();
            SetPasswordArray(passwordArray);
        }
        else
        {
            PasswordArray = Array.Empty<ObjectPassword>();
        }
        
        OnChange?.Invoke();
        return PasswordArray;
    }

    public async Task<ObjectPassword[]> UpdatePasword(string? token, ObjectPassword item)
    {
        var updatePassword = new UpdatePassword
        {
            Id = item.Id,
            Site = item.Site,
            Username = item.Username,
            Password = item.Password,
            Category = item.Category,
            CreatedAt = item.CreatedAt
        }; 
        
        var content = JsonContent.Create(updatePassword);

        var response = await _httpClient.PatchAsync(
            "https://localhost:7012/api/password/patchpassword", content);
        
        if (response.IsSuccessStatusCode)
        {
            await GetPasswordArray(token);
        }

        return null;
    }

    public async Task<ObjectPassword[]> DeletePassword(string? token, ObjectPassword item)
    {
        var response = await _httpClient.DeleteAsync(
            $"https://localhost:7012/api/password/deletepassword/{item.Id.ToString()}");

        if (response.IsSuccessStatusCode) return await GetPasswordArray(token);

        return null;
    }

    public virtual async Task<bool> CreatePassword(string? token, ObjectPasswordForm item)
    {
        var createPassword = new CreatePassword
        {
            Token = token,
            ObjectPasswordForm = item,
        };
        
        var json = JsonSerializer.Serialize(createPassword);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PostAsync(
            "https://localhost:7012/api/password/postpassword", content);

        if (response.IsSuccessStatusCode)
        {
            await GetPasswordArray(token);
            return true;
        }

        return false;
    }
}