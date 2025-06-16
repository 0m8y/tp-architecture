using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;
using GestionHotel.Application.Services;

namespace GestionHotel.Application.UseCases.Clients;

public class CreateClient
{
    private readonly IClientRepository _clientRepository;

    public CreateClient(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public Guid Execute(string name, string email, string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password is required.", nameof(password));

        var hashedPassword = PasswordHasher.Hash(password);

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (_clientRepository.ExistsByEmail(email))
            throw new InvalidOperationException("A client with this email already exists.");

        var client = new Client
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email,
            Password = hashedPassword
        };

        _clientRepository.Add(client);
        return client.Id;
    }
}
