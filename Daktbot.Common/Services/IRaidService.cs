using Daktbot.Common.Entities;
using Daktbot.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Services
{
    public interface IRaidService
    {
        Task<Result<ChannelRaid, RequestError>> GetRaidForChannel(uint channelId, string raidId);

        Task<Result<PaginatedResult<ChannelRaid>, RequestError>> GetRaidsForChannel(uint channelId);

        Task<Result<ChannelRaid, RequestError>> UpsertRaid(ChannelRaid raid);
    }
}
