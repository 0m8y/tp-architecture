using GestionHotel.Application.Services;
using Xunit;

public class PasswordHasherTests
{
    [Fact]
    public void Hash_And_Verify_ReturnsTrue_ForCorrectPassword()
    {
        var password = "MySecurePassword123!";
        var hash = PasswordHasher.Hash(password);

        var isValid = PasswordHasher.Verify(password, hash);

        Assert.True(isValid);
    }

    [Fact]
    public void Verify_ReturnsFalse_ForWrongPassword()
    {
        var password = "MySecurePassword123!";
        var wrongPassword = "WrongPassword!";
        var hash = PasswordHasher.Hash(password);

        var isValid = PasswordHasher.Verify(wrongPassword, hash);

        Assert.False(isValid);
    }
}
