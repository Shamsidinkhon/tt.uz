using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Helpers
{
    public class DateHelper
    {
        public static DateTime GetDate() {
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("West Asia Standard Time"));
        }
        public static DateTime AddDay(int day)
        {
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("West Asia Standard Time")).AddDays(day);
        }
        public static DateTime AddMinut(int minut)
        {
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("West Asia Standard Time")).AddMinutes(minut);
        }
    }
}
