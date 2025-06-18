using GestionHotel.Application.Services;
using GestionHotel.Application.Settings;
using GestionHotel.Application.UseCases.Clients;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using System;
using Xunit;

namespace GestionHotel.Tests.UseCases.Clients;

public class LoginClientTests
{
    private readonly Mock<IClientRepository> _mockRepo;
    private readonly IOptions<JwtSettings> _jwtOptions;
    private readonly LoginClient _useCase;

    public LoginClientTests()
    {
        _mockRepo = new Mock<IClientRepository>();

        _jwtOptions = Options.Create(new JwtSettings
        {
            Key = "THIS_IS_A_FAKE_KEY_FOR_TESTING_PURPOSE_ONLY"
        });

        _useCase = new LoginClient(_mockRepo.Object, _jwtOptions);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenEmailNotFound()
    {
        _mockRepo.Setup(r => r.GetByEmail("notfound@mail.com")).Returns((Client?)null);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            _useCase.Execute("notfound@mail.com", "password")
        );

        Assert.Equal("Invalid email or password.", ex.Message);
    }

    [Fact]
    public void Execute_ShouldThrow_WhenPasswordInvalid()
    {
        var client = new Client
        {
            Id = Guid.NewGuid(),
            Email = "user@mail.com",
            Password = PasswordHasher.Hash("correctpassword"),
            Role = Role.Client
        };

        _mockRepo.Setup(r => r.GetByEmail("user@mail.com")).Returns(client);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            _useCase.Execute("user@mail.com", "wrongpassword")
        );

        Assert.Equal("Invalid email or password.", ex.Message);
    }

    [Fact]
    public void Execute_ShouldReturnJwtToken_WhenCredentialsValid()
    {
        var client = new Client
        {
            Id = Guid.NewGuid(),
            Email = "user@mail.com",
            Password = PasswordHasher.Hash("password123"),
            Role = Role.Client
        };

        _mockRepo.Setup(r => r.GetByEmail("user@mail.com")).Returns(client);

        string token = _useCase.Execute("user@mail.com", "password123");

        Assert.False(string.IsNullOrWhiteSpace(token));
        Assert.Contains(".", token);
    }
}
