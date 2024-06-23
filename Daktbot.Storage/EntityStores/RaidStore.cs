using Daktbot.Common.Entities;
using Daktbot.Common.Stores;
using Daktbot.Storage.Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Storage.EntityStores
{
    [StorageContainer("Raids", PartitionKey = "channelId")]
    internal class RaidStore : BaseEntityStore<ChannelRaid>, IRaidStore
    {
        public RaidStore(
            ICosmosDBHelper dbHelper,
            ILogger<IRaidStore> logger) : base(dbHelper, logger) { }
    }
}
