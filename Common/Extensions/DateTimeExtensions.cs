namespace Common.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToEuropeanStandardDisplay(this DateTime dateTime)
            => $"{dateTime.Day.AddZero()}-{dateTime.Month.AddZero()}-{dateTime.Year}";

        public static string ToUsStandardDisplay(this DateTime dateTime)
            => $"{dateTime.Month.AddZero()}-{dateTime.Day.AddZero()}-{dateTime.Year}";

        private static string AddZero(this int monthOrDay) => monthOrDay < 10 ? $"0{monthOrDay}" : monthOrDay.ToString();
    }
}
