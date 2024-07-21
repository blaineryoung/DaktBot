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

namespace Daktbot.Discord.Core.Commands
{
    internal class DeleteRaid : AbstractDiscordCommand
    {
        private readonly IRaidService raidService;
        private readonly IPersonService personService;

        internal override ILogger Logger { get; }

        internal override string Name => "delete-raid";

        internal const string RaidIdOption = "raidid";

        internal override string Description => "Removes a raid from the channel's schedule.";

        public DeleteRaid(
            IRaidService raidService,
            ILogger<AddRaid> logger)
        {
            this.raidService = raidService;
            Logger = logger;
        }

        internal override async Task HandleCommand(SocketSlashCommand command)
        {
            using (Logger.BeginScope("Handling delete raid command for user {user}", command.User.GlobalName))
            {
                ulong channelId = command.Channel.Id;
                string? raidId =  command.Data.Options.FirstOrDefault(x => string.Equals(RaidIdOption, x.Name)).Value as string;

                if (string.IsNullOrWhiteSpace(raidId))
                {
                    Logger.LogError("Delete raid - user {user} gave a bad raid id", command.User.GlobalName);
                    await command.RespondAsync("We couldn't delete the raid.  Please bug Dakt");
                    return;
                }

                Result<HttpStatusCode, RequestError> deleteRaidResult = await raidService.DeleteRaid(channelId, raidId);
                if (false == deleteRaidResult.Match(
                    m => true,
                    error => Logger.LogRequestError(error, "CouldNotDeleteRaid")))
                {
                    await command.RespondAsync("We couldn't delete the raid.  Please bug Dakt");
                    return;
                }

                await command.RespondAsync($"Raid {raidId} deleted");
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
                raidIdOption = raidIdOption.WithName("raidid").WithDescription("Id of the raid to delete").WithRequired(true).WithType(ApplicationCommandOptionType.String);

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
