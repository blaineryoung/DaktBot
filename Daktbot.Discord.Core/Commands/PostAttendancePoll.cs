using Daktbot.Common.Services;
using Daktbot.Common.Stores;
using Daktbot.Discord.Core.Client;
using Discord.Net;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Daktbot.Common.Results;
using Daktbot.Common.Entities;
using Daktbot.Common.Utilities;
using Microsoft.Extensions.Options;
using System.Net;
using Daktbot.Discord.Core.Spotlets;

namespace Daktbot.Discord.Core.Commands
{
    internal class PostAttendancePoll : AbstractDiscordCommand
    {
        private readonly IPersonService personService;
        private readonly IPostRaidPoll poller;

        internal override ILogger Logger { get; }

        internal override string Name => "post-poll";

        internal const string RaidIdOption = "raidid";

        internal override string Description => "Posts a poll asking who will attend the specified raid.";

        public PostAttendancePoll(
            IPostRaidPoll poller,
            ILogger<PostAttendancePoll> logger)
        {
            this.poller = poller;
            Logger = logger;
        }

        internal override async Task HandleCommand(SocketSlashCommand command)
        {
            using (Logger.BeginScope("Handling post raid command for user {user}", command.User.GlobalName))
            {
                ulong channelId = command.Channel.Id;
                string? raidId =  command.Data.Options.FirstOrDefault(x => string.Equals(RaidIdOption, x.Name)).Value as string;

                if (string.IsNullOrWhiteSpace(raidId))
                {
                    Logger.LogError("Delete raid - user {user} gave a bad raid id", command.User.GlobalName);
                    await command.RespondAsync("We couldn't delete the raid.  Please bug Dakt");
                    return;
                }

                DateTime? raidTime = null;
                Result<DateTime, RequestError> postPollResult = await poller.PostPoll(channelId, raidId);
                if (false == postPollResult.Match(
                    t => { raidTime = t; return true; },
                    error => Logger.LogRequestError(error, "Could not post poll for raid")))
                {
                    await command.RespondAsync("We couldn't post a poll for the raid, bug Dakt");
                    return;
                }

                await command.RespondAsync($"The next raid is in <t:{Convert.ToInt32(raidTime.Value.Subtract(DateTime.UnixEpoch).TotalSeconds)}:R>");
            }
        }

        internal override async Task Register(IDiscordBotClient client, SocketGuild guild)
        {
            using (Logger.BeginScope("Registering command {command}", Name))
            {
                SlashCommandBuilder command = new SlashCommandBuilder();
                command.WithName(Name.ToLower());
                command.WithDescription(Description);

                SlashCommandOptionBuilder raidIdOption = new SlashCommandOptionBuilder();
                raidIdOption = raidIdOption.WithName("raidid").WithDescription("Id of the raid to post a poll for").WithRequired(true).WithType(ApplicationCommandOptionType.String);

                command.AddOption(raidIdOption);

                try
                {
                    await guild.CreateApplicationCommandAsync(command.Build());
                    //await client.Client.CreateGlobalApplicationCommandAsync(command.Build());
                }
                catch (ApplicationCommandException e)
                {
                    Logger.LogError(e, "Could not register command {command}", Name);
                }
            }
        }
    }
}
