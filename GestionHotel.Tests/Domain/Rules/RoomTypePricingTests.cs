using GestionHotel.Domain.Enums;
using GestionHotel.Domain.Rules;
using Xunit;
using System;

namespace GestionHotel.Tests.Domain.Rules
{
    public class RoomTypePricingTests
    {
        [Theory]
        [InlineData(RoomType.Single, 80)]
        [InlineData(RoomType.Double, 120)]
        [InlineData(RoomType.Suite, 250)]
        public void GetPrice_ReturnsExpectedPrice(RoomType type, decimal expected)
        {
            var result = RoomTypePricing.GetPrice(type);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void GetPrice_InvalidRoomType_ThrowsException()
        {
            var invalidType = (RoomType)(-1);

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => RoomTypePricing.GetPrice(invalidType));
            Assert.Contains("Tarif inconnu", ex.Message);
        }
    }
}
