using Daktbot.Discord.Core.Client;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core.Commands
{
    internal abstract class AbstractDiscordCommand
    {
        internal abstract ILogger Logger { get; }

        internal abstract string Name { get; }

        internal abstract string Description { get; }

        /// <summary>
        /// Will only be called if the command doesn't exist.
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        internal virtual async Task Register(IDiscordBotClient client, SocketGuild guild)
        {
            using (Logger.BeginScope("Registering command {command}", Name))
            {
                SlashCommandBuilder command = new SlashCommandBuilder();
                command.WithName(Name);
                command.WithDescription(Description);

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

        internal abstract Task HandleCommand(SocketSlashCommand command);

        internal async Task HandleCommandWrapper(SocketSlashCommand command)
        {
            using (Logger.BeginScope("Executing command {command}", Name))
            {
                await HandleCommand(command);
            }
        }
    }
}
