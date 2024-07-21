using Daktbot.Common.Entities;
using Daktbot.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Services
{
    public interface IRaidService
    {
        Task<Result<ChannelRaid, RequestError>> GetRaidForChannel(ulong channelId, string raidId);

        Task<Result<PaginatedResult<ChannelRaid>, RequestError>> GetRaidsForChannel(ulong channelId);

        Task<Result<ChannelRaid, RequestError>> UpsertRaid(ChannelRaid raid);

        Task<Result<string,  RequestError>> GetRaidTimesString(ChannelRaid raid, IReadOnlyDictionary<TimeZoneInfo, string> playerMappings);

        Task<Result<TimeSpan, RequestError>> GetTimeToNextRaid(ChannelRaid raid);

        Task<Result<string, RequestError>> PrintTimeToNextRaid(IEnumerable<ChannelRaid> raids);

        Task<Result<HttpStatusCode, RequestError>> DeleteRaid(ulong channelId, string raidId);
    }
}
