using Daktbot.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core.Spotlets
{
    public interface IPostRaidPoll
    {
        Task<Result<DateTime, RequestError>> PostPoll(ulong channelId, string raidId);
    }
}
