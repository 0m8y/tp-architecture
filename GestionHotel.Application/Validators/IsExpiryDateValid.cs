using System.Globalization;

namespace GestionHotel.Application.Validators
{
    public static class CardValidator
    {
        public static bool IsExpiryDateValid(string expiryDate)
        {
            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.Calendar.TwoDigitYearMax = 2099; // Interprète "50" comme 2050

            if (!DateTime.TryParseExact("01/" + expiryDate, "dd/MM/yy", culture, DateTimeStyles.None, out var date))
                return false;

            return date >= DateTime.Today;
        }
    }
}
