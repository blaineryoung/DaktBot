using Daktbot.Common.Results;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core.Logic
{
    public interface IRaidPollCreator
    {
        Task<Result<HttpStatusCode, RequestError>> PostPoll(ulong channelId, string? raidId = null);

        Task<Result<PollProperties, RequestError>> GetPoll(ulong channelId, string? raidId = null);
    }
}
