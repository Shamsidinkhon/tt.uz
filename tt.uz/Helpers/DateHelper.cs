using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Helpers
{
    public class DateHelper
    {
        public static DateTime GetDate()
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Asia/Tashkent"));
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("West Asia Standard Time"));
        }
        public static DateTime AddDay(int day)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Asia/Tashkent")).AddDays(day);
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("West Asia Standard Time")).AddDays(day);
        }
        public static DateTime AddMinut(int minut)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux))
                return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Asia/Tashkent")).AddMinutes(minut);
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("West Asia Standard Time")).AddMinutes(minut);
        }
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = getEpoch();
            dtDateTime = dtDateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public static Int64 GetTotalMilliseconds()
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = getEpoch();
            TimeSpan diff = GetDate() - dtDateTime;
            return Convert.ToInt64(diff.TotalMilliseconds);
        }

        public static double GetTotalSeconds()
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = getEpoch();
            TimeSpan diff = GetDate() - dtDateTime;
            return diff.TotalSeconds;
        }

        public static double GetTotalSecondsByDate(DateTime date)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = getEpoch();
            TimeSpan diff = date - dtDateTime;
            return diff.TotalSeconds;
        }

        public static Int64 GetTotalMillisecondsByDate(DateTime date)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = getEpoch();
            TimeSpan diff = date - dtDateTime;
            return Convert.ToInt64(diff.TotalMilliseconds) < 0 ? 0 : Convert.ToInt64(diff.TotalMilliseconds);
        }

        public static DateTime getEpoch(){
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }
    }
}
