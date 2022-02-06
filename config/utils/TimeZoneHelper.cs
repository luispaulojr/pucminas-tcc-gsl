using System;
using System.Globalization;

namespace config.utils
{
    public static class TimeZoneHelper
    {
        #if DEBUG
            private static readonly TimeZoneInfo LocalTimeZoneId = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
        #else
            private static readonly TimeZoneInfo LocalTimeZoneId = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");
        #endif

        public static DateTime BrazilCurrentTime
        {
            get { return TimeZoneInfo.ConvertTime(DateTime.Now, LocalTimeZoneId); }
        }

        public static DateTime ParseToBrazilCurrentTime(String dateTime) => TimeZoneInfo.ConvertTime(DateTime.Parse(dateTime), LocalTimeZoneId);

        public static DateTime ParseToBrazilCurrentTime(DateTime dateTime, bool truncateHMS)
        {
            DateTime hmsZero = BrazilCurrentTime;

            if(truncateHMS)
                hmsZero = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0);
            
            var date = String.Format("{0:yyyy-MM-dd}", dateTime);
            var hms = String.Format("{0:HH:mm:ss}", hmsZero);

            return DateTime.ParseExact(date + " " + hms + ",000", "yyyy-MM-dd HH:mm:ss,fff", new CultureInfo("pt-BR"));
        }
    }
}