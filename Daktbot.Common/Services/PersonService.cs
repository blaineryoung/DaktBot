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
    internal class PersonService : IPersonService
    {
        private readonly IPersonStore personStore;
        private readonly ILogger<IPersonService> logger;
        private readonly IEnumerable<TimeZoneInfo> timeZones;

        public PersonService(
            IPersonStore personStore,
            ILogger<IPersonService> logger)
        {
            this.timeZones = TimeZoneInfo.GetSystemTimeZones();
            this.personStore = personStore;
            this.logger = logger;
        }

        public async Task<Result<ChannelPerson, RequestError>> GetPersonForChannel(ulong channelId, ulong userId)
        {
            return await personStore.Get(userId.ToString(), channelId.ToString());
        }

        public async Task<Result<PaginatedResult<ChannelPerson>, RequestError>> GetPersonsForChannel(ulong channelId)
        {
            return await personStore.GetAll(channelId.ToString(), null);
        }

        public async Task<Result<ChannelPerson, RequestError>> UpsertPerson(ChannelPerson person)
        {
            return await personStore.Upsert(person, person.ChannelId);
        }

        public async Task<Result<IReadOnlyDictionary<TimeZoneInfo, string>, RequestError>> GetPlayerTimezoneMappings(string channelIdString)
        {
            ulong channelId = 0;
            if (false == ulong.TryParse(channelIdString, out channelId))
            {
                return new RequestError($"Could not parse {channelIdString} as a ulong", System.Net.HttpStatusCode.BadRequest);
            }

            Result<PaginatedResult<ChannelPerson>, RequestError> getPlayersResult = await this.GetPersonsForChannel(channelId);
            PaginatedResult<ChannelPerson> personList = null;
            if (false == getPlayersResult.Match<bool>(
                p => { personList = p; return true; },
                error => { logger.LogRequestError(error, "Could not get players for channel {channelid}", channelId); return false; }))
            {
                return getPlayersResult.CastError<IReadOnlyDictionary<TimeZoneInfo, string>>();
            }

            Dictionary<TimeZoneInfo, string> playerTimezoneMappings = new Dictionary<TimeZoneInfo, string>();
            foreach (ChannelPerson person in personList.Value)
            {
                TimeZoneInfo? personsTimeZone = this.timeZones.Where(y => 0 == string.Compare(y.Id, person.TimezoneId)).FirstOrDefault();
                if (null == personsTimeZone)
                {
                    return new RequestError($"Could not find time zone {person.TimezoneId} for player {person.Id}", System.Net.HttpStatusCode.BadRequest);
                }

                string userList = string.Empty;
                if (false == playerTimezoneMappings.TryGetValue(personsTimeZone, out userList))
                {
                    userList = person.DisplayName;
                    playerTimezoneMappings.Add(personsTimeZone, userList);
                }
                else
                {
                    userList += $", {person.DisplayName}";
                    playerTimezoneMappings[personsTimeZone] = userList;
                }
            }

            return playerTimezoneMappings;
        }
    }
}
