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
    internal class PersonService : IPersonService
    {
        private readonly IPersonStore personStore;
        private readonly ILogger<IPersonService> logger;

        public PersonService(
            IPersonStore personStore,
            ILogger<IPersonService> logger)
        {
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
    }
}
