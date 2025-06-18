using Microsoft.AspNetCore.Components;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

public class AuthService
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService _localStorage;
    private readonly AuthenticationStateProvider _authStateProvider;

    public AuthService(HttpClient http, ILocalStorageService localStorage, AuthenticationStateProvider authStateProvider)
    {
        _http = http;
        _localStorage = localStorage;
        _authStateProvider = authStateProvider;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var loginRequest = new { Email = email, Password = password };
        var response = await _http.PostAsJsonAsync("https://localhost:7208/api/v1/clients/login", loginRequest);

        if (!response.IsSuccessStatusCode)
            return false;

        var token = await response.Content.ReadAsStringAsync();
        await _localStorage.SetItemAsync("authToken", token);

        // ✅ Cast sécurisé
        if (_authStateProvider is CustomAuthStateProvider customProvider)
        {
            customProvider.NotifyUserAuthentication(token);
        }

        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return true;
    }

    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync("authToken");

        if (_authStateProvider is CustomAuthStateProvider customProvider)
        {
            customProvider.NotifyUserLogout();
        }

        _http.DefaultRequestHeaders.Authorization = null;
    }
}
