using System.Net.Http.Json;
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

    public async Task<ObjectPassword[]> GetPasswordArray()
    {
        var response = await _httpClient.GetAsync("https://localhost:7012/api/password/getpassword");
        
        if (response.IsSuccessStatusCode)
        {
            ObjectPassword[] passwordArray = await response.Content.ReadFromJsonAsync<ObjectPassword[]>();
            SetPasswordArray(passwordArray);
        }
        else
        {
            PasswordArray = Array.Empty<ObjectPassword>();
        }
        
        OnChange?.Invoke();
        return PasswordArray;
    }

    public async Task<ObjectPassword[]> UpdatePasword(ObjectPassword item)
    {
        var content = JsonContent.Create(item);

        var response = await _httpClient.PatchAsync(
            "https://localhost:7012/api/password/patchpassword", content);
        
        if (response.IsSuccessStatusCode)
        {
            await GetPasswordArray();
        }

        return null;
    }

    public async Task<ObjectPassword[]> DeletePassword(ObjectPassword item)
    {
        var response = await _httpClient.DeleteAsync(
            $"https://localhost:7012/api/password/deletepassword/{item.Id.ToString()}");

        if (response.IsSuccessStatusCode) return await GetPasswordArray();

        return null;
    }
}