using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Application.Services;

namespace GestionHotel.Application.UseCases.Clients;

public class LoginClient
{
    private readonly IClientRepository _clientRepository;

    public LoginClient(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public string Execute(string email, string password)
    {
        var client = _clientRepository.GetByEmail(email);
        if (client == null)
            throw new InvalidOperationException("Invalid email or password.");

        var isValid = PasswordHasher.Verify(password, client.Password);
        if (!isValid)
            throw new InvalidOperationException("Invalid email or password.");

        var token = Guid.NewGuid().ToString(); // simple token simulé
        SessionStore.Tokens[token] = client.Id;

        return token;
    }
}
