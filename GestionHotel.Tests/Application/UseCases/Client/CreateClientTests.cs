using Xunit;
using Moq;
using System;
using GestionHotel.Application.UseCases.Clients;
using GestionHotel.Domain.Entities;
using GestionHotel.Domain.Interfaces;

namespace GestionHotel.Tests.UseCases.Clients;

public class CreateClientTests
{
    private readonly Mock<IClientRepository> _mockRepo;
    private readonly CreateClient _useCase;

    public CreateClientTests()
    {
        _mockRepo = new Mock<IClientRepository>();
        _useCase = new CreateClient(_mockRepo.Object);
    }

    [Fact]
    public void Execute_ShouldThrowException_WhenPasswordIsEmpty()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            _useCase.Execute("John", "john@mail.com", "")
        );
        Assert.Equal("Password is required. (Parameter 'password')", ex.Message);
    }

    [Fact]
    public void Execute_ShouldThrowException_WhenNameIsEmpty()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            _useCase.Execute("", "john@mail.com", "password123")
        );
        Assert.Equal("Name is required. (Parameter 'name')", ex.Message);
    }

    [Fact]
    public void Execute_ShouldThrowException_WhenEmailIsEmpty()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            _useCase.Execute("John", "", "password123")
        );
        Assert.Equal("Email is required. (Parameter 'email')", ex.Message);
    }

    [Fact]
    public void Execute_ShouldThrowException_WhenEmailAlreadyExists()
    {
        _mockRepo.Setup(r => r.ExistsByEmail("john@mail.com")).Returns(true);

        var ex = Assert.Throws<InvalidOperationException>(() =>
            _useCase.Execute("John", "john@mail.com", "password123")
        );
        Assert.Equal("A client with this email already exists.", ex.Message);
    }

    [Fact]
    public void Execute_ShouldCreateClient_WhenDataIsValid()
    {
        _mockRepo.Setup(r => r.ExistsByEmail(It.IsAny<string>())).Returns(false);

        Guid result = _useCase.Execute("John", "john@mail.com", "password123");

        _mockRepo.Verify(r => r.Add(It.Is<Client>(c =>
            c.Name == "John" &&
            c.Email == "john@mail.com" &&
            !string.IsNullOrWhiteSpace(c.Password)
        )), Times.Once);

        Assert.NotEqual(Guid.Empty, result);
    }
}
