using System.Globalization;

namespace Infrastructure.Helppers
{
    public static class ConcurrencyFormat
    {
        public static string FormatCurrency(double price)
        {
            CultureInfo vietnameseCulture = new CultureInfo("vi-VN");
            return string.Format(vietnameseCulture, "{0:C0}", price);
        }
    }
}
