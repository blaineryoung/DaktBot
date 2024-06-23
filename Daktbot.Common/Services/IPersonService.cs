using Daktbot.Common.Entities;
using Daktbot.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Services
{
    public interface IPersonService
    {
        Task<Result<ChannelPerson, RequestError>> GetPersonForChannel(ulong channelId, ulong userId);

        Task<Result<PaginatedResult<ChannelPerson>, RequestError>> GetPersonsForChannel(ulong channelId);

        Task<Result<ChannelPerson, RequestError>> UpsertPerson(ChannelPerson person);
    }
}
