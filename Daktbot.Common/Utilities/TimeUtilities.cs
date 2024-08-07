﻿using Daktbot.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Daktbot.Common.Utilities
{
    public static class TimeUtilities
    {
        private static Regex IsTime = new Regex("\\b((0?[1-9]|1[012])([:.][0-5][0-9])?(\\s?[AaPp][Mm])|([01]?[0-9]|2[0-3])([:.][0-5][0-9]))\\b");
        private static Regex TimeParser = new Regex("((1[0-2]|0?[1-9]):?([0-5][0-9])?\\s*?([AaPp][Mm]))");

        public static bool ContainsTime(string content)
        {
            MatchCollection mc;

            mc = IsTime.Matches(content);

            return mc.Count != 0;
        }

        public static DateTime GetNextTimeForDayOfWeek(DateTime baseTime, DayOfWeek dayOfWeek)
        {
            int daysTillRaid = dayOfWeek >= baseTime.DayOfWeek ? (dayOfWeek - baseTime.DayOfWeek) : (7 - (int)baseTime.DayOfWeek + (int)dayOfWeek);
            return baseTime.AddDays(daysTillRaid);
        }

        public static Result<DateTime,RequestError> GetTimeFromText(string timeString, TimeZoneInfo sourceTimeZone)
        {
            // Determine if there was a match.
            MatchCollection mc = TimeParser.Matches(timeString);
            if (mc.Count == 0)
            {
                return new RequestError($"{timeString} is not a valid time", HttpStatusCode.BadRequest);
            }

            // If there are multiple times matched, we just pick the first.  Could do th is in 
            // a loop if we wanted to be robust.
            int userHour;
            int userMinutes;
            string userMeridian;


            if (mc[0].Groups.Count < 5)
            {
                return new RequestError($"{timeString} is not a valid time", HttpStatusCode.BadRequest);
            }

            // Parse out the entered time.
            userHour = int.Parse(mc[0].Groups[2].Value);
            userMinutes = string.IsNullOrEmpty(mc[0].Groups[3].Value) ? 0 : int.Parse(mc[0].Groups[3].Value);
            userMeridian = mc[0].Groups[4].Value;

            // Deal with wierdness around am/pm
            if (0 == string.Compare(userMeridian, "pm", StringComparison.OrdinalIgnoreCase))
            {
                if (userHour != 12)
                {
                    userHour += 12;
                }
            }
            else
            {
                if (userHour == 12)
                {
                    userHour = 0;
                }
            }

            // Bit of a hack to deal with the c# time system.  Time zones/daylight savings time are dependent on date, so just
            // assume today's date.
            DateTime userTimeToday = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc, sourceTimeZone);
            DateTime userTime = new DateTime(userTimeToday.Year, userTimeToday.Month, userTimeToday.Day, userHour, userMinutes, 0);

            userTime.AddHours(userHour);
            userTime.AddMinutes(userMinutes);

            return userTime;
        }

        public static StringBuilder PrintUserTimes(DateTime sourceTime, TimeZoneInfo sourceTimeZone, IReadOnlyDictionary<TimeZoneInfo, string> displayMappings)
        {
            StringBuilder sb = new StringBuilder();

            // Cycle through every group and print out the appropriate time for them.
            foreach (KeyValuePair<TimeZoneInfo, string> displayer in displayMappings)
            {
                DateTime local = TimeZoneInfo.ConvertTime(sourceTime, sourceTimeZone, displayer.Key);
                if (local.DayOfWeek == sourceTime.DayOfWeek)
                {
                    sb.AppendLine($"**{displayer.Value}** - {local.ToShortTimeString()}");
                }
                else
                {
                    sb.AppendLine($"**{displayer.Value}** - {local.ToShortTimeString()} ({local.DayOfWeek})");
                }
            }

            return sb;
        }

        public static String PrettyPrintTimeSpan(TimeSpan span) 
        {
            String timeTillRaid = String.Empty;

            if (0 != span.Days)
            {
                timeTillRaid = $"{span.Days} days, {span.Hours} hours, {span.Minutes} minutes";
            }
            else if (0 != span.Hours)
            {
                timeTillRaid = $"{span.Hours} hours, {span.Minutes} minutes";
            }
            else if (0 != span.Minutes)
            {
                timeTillRaid = $"{span.Minutes} minutes";
            }
            else
            {
                timeTillRaid = "NOW!";
            }
            return timeTillRaid;
        }
    }
}
