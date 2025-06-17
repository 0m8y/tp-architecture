using GestionHotel.Domain.Interfaces;
using GestionHotel.Application.Services;

using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Options;
using System.Text;
using GestionHotel.Application.Settings;


namespace GestionHotel.Application.UseCases.Clients;

public class LoginClient
{
    private readonly IClientRepository _clientRepository;
    private readonly string _jwtKey;

    public LoginClient(IClientRepository clientRepository, IOptions<JwtSettings> jwtOptions)
    {
        _clientRepository = clientRepository;
        _jwtKey = jwtOptions.Value.Key;
    }

    public string Execute(string email, string password)
    {
        var client = _clientRepository.GetByEmail(email);
        if (client == null)
            throw new InvalidOperationException("Invalid email or password.");

        var isValid = PasswordHasher.Verify(password, client.Password);
        if (!isValid)
            throw new InvalidOperationException("Invalid email or password.");

        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtKey)); // stocké en config
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] {
            new Claim(ClaimTypes.NameIdentifier, client.Id.ToString()),
            new Claim(ClaimTypes.Email, client.Email)
        }),
            Expires = DateTime.UtcNow.AddHours(2),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
