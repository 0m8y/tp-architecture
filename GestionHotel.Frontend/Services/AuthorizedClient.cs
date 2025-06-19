using Blazored.LocalStorage;
using System.Net.Http.Headers;

public class AuthorizedHttpClient
{
    private readonly HttpClient _client = new();
    public HttpClient Client => _client;

    public async Task InitAsync(ILocalStorageService localStorage)
    {
        var token = await localStorage.GetItemAsync<string>("authToken");
        if (!string.IsNullOrWhiteSpace(token))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
