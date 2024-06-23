using Daktbot.Common.Entities;
using Daktbot.Common.Results;
using Daktbot.Common.Stores;
using Daktbot.Common.Utilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Services
{
    internal class RaidService : IRaidService
    {
        private readonly IRaidStore raidStore;
        private readonly ILogger<IRaidService> logger;

        private readonly IEnumerable<TimeZoneInfo> timeZones;

        public RaidService(
            IRaidStore raidStore,
            ILogger<IRaidService> logger)
        {
            this.timeZones = TimeZoneInfo.GetSystemTimeZones();
            this.raidStore = raidStore;
            this.logger = logger;
        }

        public async Task<Result<ChannelRaid, RequestError>> GetRaidForChannel(ulong channelId, string raidId)
        {
            return await raidStore.Get(raidId, channelId.ToString());
        }

        public async Task<Result<PaginatedResult<ChannelRaid>, RequestError>> GetRaidsForChannel(ulong channelId)
        {
            return await raidStore.GetAll(channelId.ToString());
        }

        public async Task<Result<string, RequestError>> GetRaidTimesString(ChannelRaid raid, IReadOnlyDictionary<TimeZoneInfo, string> playerMappings)
        {
            StringBuilder raidTimes = new StringBuilder();
            raidTimes.AppendLine(raid.Day);

            TimeZoneInfo timeZone = this.timeZones.Where(x => 0 == string.Compare(x.Id, raid.TimezoneId)).First();
            Result<DateTime, RequestError> getRaidTimeResult = TimeUtilities.GetTimeFromText(raid.Time, timeZone);
            DateTime r = DateTime.MaxValue;

            if (false == getRaidTimeResult.Match<bool>(
                t => { r = t; return true; },
                error => { this.logger.LogRequestError(error, "Could not get next time for raid {raid}", raid.Id); return false; }
                ))
            {
                return getRaidTimeResult.CastError<string>();
            }

            DayOfWeek raidDay;
            if (false == Enum.TryParse<DayOfWeek>(raid.Day, true, out raidDay)) 
            {
                return new RequestError($"Could not parse day of week {raid.Day}", System.Net.HttpStatusCode.BadRequest);
            }

            r = TimeUtilities.GetNextTimeForDayOfWeek(r, raidDay);
            StringBuilder sb = TimeUtilities.PrintUserTimes(r, timeZone, playerMappings);

            return sb.ToString();
        }

        public async Task<Result<string, RequestError>> PrintTimeToNextRaid(IEnumerable<ChannelRaid> raids)
        {
            TimeSpan nextRaid = TimeSpan.MaxValue;

            foreach (ChannelRaid raid in raids)
            {
                TimeSpan timeToRaid = TimeSpan.MaxValue;

                Result<TimeSpan, RequestError> getTimeResult = await GetTimeToNextRaid(raid);
                if (false == getTimeResult.Match<bool>(
                    t => { timeToRaid = t; return true; },
                    error => { logger.LogRequestError(error, "Could not get time to next raid {raid}", raid.Id); return false; }))
                {
                    return getTimeResult.CastError<string>();
                }

                if (timeToRaid < nextRaid) 
                {
                    nextRaid = timeToRaid;
                }
            }

            String timeTillRaid = String.Empty;

            if (nextRaid < TimeSpan.MaxValue)
            {
                if (0 != nextRaid.Days)
                {
                    timeTillRaid = $"{nextRaid.Days} days, {nextRaid.Hours} hours, {nextRaid.Minutes} minutes";
                }
                else if (0 != nextRaid.Hours)
                {
                    timeTillRaid = $"{nextRaid.Hours} hours, {nextRaid.Minutes} minutes";
                }
                else if (0 != nextRaid.Minutes)
                {
                    timeTillRaid = $"{nextRaid.Minutes} minutes";
                }
                else
                {
                    timeTillRaid = "NOW!";
                }
                return timeTillRaid;
            }
            else
            {
                return new RequestError("Could not calculate time to next raid", System.Net.HttpStatusCode.InternalServerError);
            }
        }

        public async Task<Result<TimeSpan, RequestError>> GetTimeToNextRaid(ChannelRaid raid)
        {
            TimeZoneInfo timeZone = this.timeZones.Where(x => 0 == string.Compare(x.Id, raid.TimezoneId)).First();
            Result<DateTime, RequestError> getRaidTimeResult = TimeUtilities.GetTimeFromText(raid.Time, timeZone);
            DateTime? r = null;

            if (false == getRaidTimeResult.Match<bool>(
                t => { r = t; return true; },
                error => { this.logger.LogRequestError(error, "Could not get next time for raid {raid}", raid.Id); return false; }
                )) 
            {
                return getRaidTimeResult.CastError<TimeSpan>();
            }

            DateTime raidTime = r.Value;

            DayOfWeek raidDay;
            if (raidTime != null && false != Enum.TryParse<DayOfWeek>(raid.Day, out raidDay))
            {
                int daysTillRaid = raidDay >= raidTime.DayOfWeek ? (raidDay - raidTime.DayOfWeek) : (7 - (int)raidTime.DayOfWeek + (int)raidDay);
                raidTime = raidTime.AddDays(daysTillRaid);

                DateTime utcRaidTime = TimeZoneInfo.ConvertTime(raidTime, timeZone, TimeZoneInfo.Utc);

                TimeSpan ts = utcRaidTime.Subtract(DateTime.UtcNow);
                return ts;
            }
            else
            {
                return new RequestError($"Could not determine raid day of week {raid.Day}", System.Net.HttpStatusCode.BadRequest);
            }
        }

        public async Task<Result<ChannelRaid, RequestError>> UpsertRaid(ChannelRaid raid)
        {
            return await raidStore.Upsert(raid, raid.ChannelId);
        }
    }
}
