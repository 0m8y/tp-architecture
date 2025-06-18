using Blazored.LocalStorage;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;

    public CustomAuthStateProvider(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _localStorage.GetItemAsync<string>("authToken");
        var identity = string.IsNullOrWhiteSpace(token)
            ? new ClaimsIdentity()
            : new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");

        var user = new ClaimsPrincipal(identity);
        return new AuthenticationState(user);
    }

    public void NotifyUserAuthentication(string token)
    {
        var identity = new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt");
        var user = new ClaimsPrincipal(identity);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
    }

    public void NotifyUserLogout()
    {
        var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(anonymous)));
    }

    private IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
    {
        var payload = jwt.Split('.')[1];
        var jsonBytes = Convert.FromBase64String(PadBase64(payload));
        var claims = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        var result = new List<Claim>();

        foreach (var kvp in claims)
        {
            if (kvp.Key == "role")
            {
                // Un seul rôle ou liste de rôles
                if (kvp.Value.ToString().StartsWith("["))
                {
                    var roles = JsonSerializer.Deserialize<string[]>(kvp.Value.ToString());
                    result.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
                }
                else
                {
                    result.Add(new Claim(ClaimTypes.Role, kvp.Value.ToString()));
                }
            }
            else
            {
                result.Add(new Claim(kvp.Key, kvp.Value.ToString()));
            }
        }

        return result;
    }

    private static string PadBase64(string base64)
    {
        return base64.PadRight(base64.Length + (4 - base64.Length % 4) % 4, '=');
    }
}
