using GestionHotel.Application.Validators;
using Xunit;

public class CardValidatorTests
{
    [Theory]
    [InlineData("12/30")]
    [InlineData("01/50")]
    public void IsExpiryDateValid_WithValidDate_ReturnsTrue(string expiryDate)
    {
        var result = CardValidator.IsExpiryDateValid(expiryDate);
        Assert.True(result);
    }

    [Theory]
    [InlineData("01/20")]
    [InlineData("13/25")]
    [InlineData("00/25")]
    [InlineData("aa/bb")]
    public void IsExpiryDateValid_WithInvalidDate_ReturnsFalse(string expiryDate)
    {
        var result = CardValidator.IsExpiryDateValid(expiryDate);
        Assert.False(result);
    }
}
