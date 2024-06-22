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
    [StorageContainer("People", PartitionKey = "channelId")]
    internal class PersonStore : BaseEntityStore<ChannelPerson>, IPersonStore
    {
        public PersonStore(
            ICosmosDBHelper dbHelper,
            ILogger<IPersonStore> logger) : base(dbHelper, logger) { }
    }
}
