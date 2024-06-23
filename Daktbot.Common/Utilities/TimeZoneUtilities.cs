using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Utilities
{
    public static class TimeZoneUtilities
    {
        static private string[] validTimeZones =
        {
            "(UTC-08:00) Pacific Time (US & Canada)",
            "(UTC-07:00) Arizona",
            "(UTC-07:00) Mountain Time (US & Canada)",
            "(UTC-07:00) Yukon",
            "(UTC-06:00) Central Time (US & Canada)",
            "(UTC-06:00) Easter Island",
            "(UTC-06:00) Guadalajara, Mexico City, Monterrey",
            "(UTC-06:00) Saskatchewan",
            "(UTC-05:00) Eastern Time (US & Canada)",
            "(UTC-05:00) Indiana (East)",
            "(UTC+08:00) Beijing, Chongqing, Hong Kong, Urumqi",
            "(UTC+08:00) Perth",
            "(UTC+08:00) Taipei",
            "(UTC+09:00) Osaka, Sapporo, Tokyo",
            "(UTC+09:00) Pyongyang",
            "(UTC+09:00) Seoul",
            "(UTC+09:30) Darwin",
            "(UTC+10:00) Brisbane",
            "(UTC+10:00) Canberra, Melbourne, Sydney",
            "(UTC+10:00) Guam, Port Moresby",
            "(UTC+10:30) Lord Howe Island",
            "(UTC+11:00) Bougainville Island",
            "(UTC+11:00) Norfolk Island",
            "(UTC+11:00) Solomon Is., New Caledonia",
        };

        public static IEnumerable<TimeZoneInfo> GetCuratedTimeZones()
        {
            return TimeZoneInfo.GetSystemTimeZones().Where(x => validTimeZones.Contains(x.DisplayName)).ToArray();
        }
    }
}
