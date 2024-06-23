using Daktbot.Common.Entities;
using Daktbot.Common.Results;
using Daktbot.Common.Stores;
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

        public RaidService(
            IRaidStore raidStore,
            ILogger<IRaidService> logger)
        {
            this.raidStore = raidStore;
            this.logger = logger;
        }

        public async Task<Result<ChannelRaid, RequestError>> GetRaidForChannel(uint channelId, string raidId)
        {
            return await raidStore.Get(raidId, channelId.ToString());
        }

        public async Task<Result<PaginatedResult<ChannelRaid>, RequestError>> GetRaidsForChannel(uint channelId)
        {
            return await raidStore.GetAll(channelId.ToString());
        }

        public async Task<Result<ChannelRaid, RequestError>> UpsertRaid(ChannelRaid raid)
        {
            return await raidStore.Upsert(raid, raid.ChannelId);
        }
    }
}
