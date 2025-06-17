using System;
using System.Globalization;

namespace GestionHotel.Application.Validators
{
    public static class CardValidator
    {
        public static bool IsExpiryDateValid(string expiryDate)
        {
            if (!DateTime.TryParseExact("01/" + expiryDate, "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return false;

            return date >= DateTime.Today;
        }
    }
}
