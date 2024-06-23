using Daktbot.Common.Entities;
using Daktbot.Common.Results;
using Daktbot.Common.Services;
using Daktbot.Common.Utilities;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core.Commands
{
    internal class RaidTimes : AbstractDiscordCommand
    {
        private readonly IPersonService playerService;
        private readonly IRaidService raidService;

        internal override ILogger Logger { get; }

        internal override string Name => "raidtimes";

        internal override string Description => "Print out the times of the channel's raids for all time zones.";

        public RaidTimes(
            IPersonService playerService,
            IRaidService raidService,
            ILogger<RaidTimes> logger)
        {
            this.playerService = playerService;
            this.raidService = raidService;
            Logger = logger;
        }

        internal override async Task HandleCommand(SocketSlashCommand command)
        {
            StringBuilder raidTimes = new StringBuilder();

            if (command.ChannelId == null) 
            {
                Logger.LogWarning("Got a command with no guild id");
                await command.RespondAsync($"Failed...comand had no channel id");
                return;
            }

            IReadOnlyDictionary<TimeZoneInfo, string> playerMappings = null;
            Result<IReadOnlyDictionary<TimeZoneInfo, string>, RequestError> getPlayerMappingRequest = await playerService.GetPlayerTimezoneMappings(command.ChannelId.ToString());
            if (false == getPlayerMappingRequest.Match<bool>(
                pm => { playerMappings = pm; return true; },
                error => Logger.LogRequestError(error, "Could not get player mappings for channel {channelId}", command.ChannelId)))
            {
                await command.RespondAsync($"Failed...could not get player mappings.");
                return;
            }

            IEnumerable<ChannelRaid> raids = null;
            Result<PaginatedResult<ChannelRaid>, RequestError> getRaidsResult = await raidService.GetRaidsForChannel(command.ChannelId.Value);
            if (false == getRaidsResult.Match<bool>(
                r => { raids = r.Value; return true; },
                error => Logger.LogRequestError(error, "Could not get raids for channel {channel}", command.ChannelId)))
            {
                await command.RespondAsync($"Failed...could not retrieve raids for channel");
                return;
            }

            foreach (ChannelRaid raid in raids)
            {
                raidTimes.AppendLine($"{raid.Day}");

                Result<string, RequestError> getRaidTimesString = await raidService.GetRaidTimesString(raid, playerMappings);
                if (false == getRaidTimesString.Match<bool>(
                    rts => { raidTimes.Append(rts); return true; },
                    error => { Logger.LogRequestError(error, "Could not map raid times for channel {channel}", command.ChannelId); return false; }))
                {
                    await command.RespondAsync($"Failed...could not map raids for channel");
                    return;
                }
                raidTimes.AppendLine();
            }

            var printTimeToNextRaidResult = await raidService.PrintTimeToNextRaid(raids);
            if (false == printTimeToNextRaidResult.Match<bool>(
                ttr => { raidTimes.AppendLine($"Next raid is in {ttr}"); return true; },
                error => Logger.LogRequestError(error, "Could not determine next raid")))
            {
                await command.RespondAsync($"Failed... could not determine next raid");
                return;
            }       

            await command.RespondAsync(raidTimes.ToString());
        }
    }
}
