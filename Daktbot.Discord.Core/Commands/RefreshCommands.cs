using Daktbot.Discord.Core.Client;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core.Commands
{
    internal class RefreshCommands : AbstractDiscordCommand
    {
        private readonly IDiscordBotClient botClient;

        internal override ILogger Logger { get; }

        internal override string Name => "refresh-commands";

        internal override string Description => "Update and reregister all commands.";

        public RefreshCommands(
            IDiscordBotClient botClient,
            ILogger<RefreshCommands> logger)
        {
            this.botClient = botClient;
            Logger = logger;
        }

        internal override async Task HandleCommand(SocketSlashCommand command)
        {
            Logger.LogInformation("Resetting commands initiated by {user}", command.User.Id);

            foreach (SocketGuild? guild in botClient.Client.Guilds) 
            {
                await guild.DeleteApplicationCommandsAsync();
            }

            await botClient.RegisterCommands();

            await command.RespondAsync($"Successfully refreshed commands.");

            Logger.LogInformation("Done");
        }
    }
}
