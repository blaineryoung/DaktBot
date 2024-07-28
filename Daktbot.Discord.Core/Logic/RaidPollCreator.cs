using Daktbot.Common.Entities;
using Daktbot.Common.Results;
using Daktbot.Common.Services;
using Daktbot.Common.Utilities;
using Daktbot.Discord.Core.Client;
using Daktbot.Discord.Core.Commands;
using Discord;
using Discord.Rest;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core.Logic
{
    public class RaidPollCreator : IRaidPollCreator
    {
        private readonly IRaidService raidService;
        private readonly IDiscordBotClient botClient;
        private readonly ILogger<RaidPollCreator> logger;

        public RaidPollCreator(
            IRaidService raidService,
            IDiscordBotClient botClient,
            ILogger<RaidPollCreator> logger) 
        {
            this.raidService = raidService;
            this.botClient = botClient;
            this.logger = logger;
        }

        public async Task<Result<PollProperties, RequestError>> GetPoll(ulong channelId, string? raidId = null)
        {
            IEnumerable<ChannelRaid> raids = null;
            Result<PaginatedResult<ChannelRaid>, RequestError> getRaidsResult = await raidService.GetRaidsForChannel(channelId);
            if (false == getRaidsResult.Match<bool>(
                r => { raids = r.Value; return true; },
                error => logger.LogRequestError(error, "Could not get raids for channel {channel}", channelId)))
            {
                return getRaidsResult.CastError<PollProperties>();
            }

            TimeSpan? timeToNextRaid = TimeSpan.MaxValue;
            ChannelRaid? raid = raids.FirstOrDefault();

            if (string.IsNullOrWhiteSpace(raidId))
            {
                foreach (ChannelRaid cr in raids)
                {
                    Result<TimeSpan, RequestError> getTimeResult = await raidService.GetTimeToNextRaid(cr);
                    if (false == getTimeResult.Match<bool>(
                        t => { if (timeToNextRaid > t) { timeToNextRaid = t; raid = cr; }; return true; },
                        error => logger.LogRequestError(error, "Could not get time to next faid for raid {raidId} for channel {channel}", raidId, channelId)))
                    {
                        return getTimeResult.CastError<PollProperties>();
                    }
                }
            }
            else
            {
                raid = raids.Where(x => String.Equals(x.Id, raidId)).FirstOrDefault();
                if (raid != null)
                {
                    Result<TimeSpan, RequestError> getTimeResult = await raidService.GetTimeToNextRaid(raid);
                    if (false == getTimeResult.Match<bool>(
                        t => { timeToNextRaid = t; return true; },
                        error => logger.LogRequestError(error, "Could not get time to next faid for raid {raidId} for channel {channel}", raidId, channelId)))
                    {
                        return getTimeResult.CastError<PollProperties>();
                    }
                }
            }

            DateTime raidTime = DateTime.UtcNow.Add(timeToNextRaid.Value);
            string message = $"Next raid is in {TimeUtilities.PrettyPrintTimeSpan(timeToNextRaid.Value)}, are you going?";

            PollMediaProperties[] answers = {
                new PollMediaProperties { Text = "Yes"},
                new PollMediaProperties { Text = "No"},
                new PollMediaProperties { Text = "Maybe"},
            };

            var poll = new PollProperties
            {
                Question = new()
                {
                    Text = message
                },
                Duration = 8,
                Answers = answers.ToList(),
                AllowMultiselect = false,
                LayoutType = PollLayout.Default
            };

            return poll;
        }

        public async Task<Result<HttpStatusCode, RequestError>> PostPoll(ulong channelId, string? raidId = null)
        {
            ITextChannel channel = await botClient.Client.GetChannelAsync(channelId) as ITextChannel;
            if (channel == null) 
            {
                logger.LogWarning("Could not find channel {channelid}", channelId);
                return new RequestError($"Could not find channel {channelId}", HttpStatusCode.NotFound);
            }

            Result<PollProperties, RequestError> getPollResult = await GetPoll(channelId, raidId);
            PollProperties? poll = null;
            if (false == getPollResult.Match<bool>(
                p => { poll = p; return true; },
                error => logger.LogRequestError(error, "Could not get time to next faid for raid {raidId} for channel {channel}", raidId, channelId)))
            {
                return getPollResult.CastError<HttpStatusCode>();
            }

            // Send the poll to the text channel
            await channel.SendMessageAsync(poll: poll);

            return HttpStatusCode.OK;
        }
    }
}
