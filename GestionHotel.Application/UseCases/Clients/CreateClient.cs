using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;

namespace GestionHotel.Application.UseCases.Clients;

public class CreateClient
{
    private readonly IClientRepository _clientRepository;

    public CreateClient(IClientRepository clientRepository)
    {
        _clientRepository = clientRepository;
    }

    public Guid Execute(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        var client = new Client
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email
        };

        _clientRepository.Add(client);

        return client.Id;
    }
}
