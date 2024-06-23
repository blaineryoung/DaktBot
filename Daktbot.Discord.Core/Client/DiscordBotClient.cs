using Daktbot.Discord.Core.Commands;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core.Client
{
    internal class DiscordBotClient : IDiscordBotClient
    {
        private readonly ILogger<DiscordBotClient> logger;
        private readonly IServiceProvider serviceProvider;
        private readonly DiscordOptions discordOptions;

        private DiscordSocketClient? client;
        private Dictionary<string, AbstractDiscordCommand> Commands = new Dictionary<string, AbstractDiscordCommand>();

        public DiscordBotClient(
            ILogger<DiscordBotClient> logger,
            IOptions<DiscordOptions> options,
            IServiceProvider serviceProvider)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.discordOptions = options.Value;
        }

        public DiscordSocketClient Client => client;

        public async Task Start()
        {
            using (logger.BeginScope("Starting discord client"))
            {
                DiscordSocketClient client = new DiscordSocketClient();
                client.Connected += RegisterCommands;
                client.Log += Log;

                await client.LoginAsync(TokenType.Bot, discordOptions.GetToken());
                await client.StartAsync();

                this.client = client;
            }          
        }

        public async Task RegisterCommands()
        {
            IEnumerable<SocketGuild> guilds = client.Guilds;

            // Register any slash commands.
            foreach (SocketGuild guild in guilds)
            {
                using (logger.BeginScope("Registering discord commands for guild {guild}", guild))
                {
                    IReadOnlyCollection<SocketApplicationCommand> discordCommandList = await guild.GetApplicationCommandsAsync();

                    IEnumerable<Type> commands = BotUtilities.GetSubclasses(typeof(AbstractDiscordCommand));
                    foreach (Type t in commands)
                    {
                        AbstractDiscordCommand command = serviceProvider.GetRequiredService(t) as AbstractDiscordCommand;
                        if (!discordCommandList.Where(x => string.Equals(x.Name, command.Name, StringComparison.OrdinalIgnoreCase)).Any())
                        {
                            logger.LogInformation("Command {command} not present, registering it", command.Name);
                            await command.Register(this, guild);
                        }
                        else
                        {
                            logger.LogInformation("Command {command} already exists, skipping", command.Name);
                        }

                        if (!Commands.ContainsKey(command.Name))
                        {
                            Commands.Add(command.Name, command);
                        }
                    }
                }
            }

            client.SlashCommandExecuted += Client_SlashCommandExecuted;
        }

        private async Task Client_SlashCommandExecuted(SocketSlashCommand command)
        {
            AbstractDiscordCommand? c = null;
            if (false == Commands.TryGetValue(command.CommandName, out c)) 
            {
                logger.LogError("Could not find command {command}, was it orphaned", command.CommandName);
                return;
            }

            await c.HandleCommandWrapper(command);
        }

        private async Task Log(LogMessage msg)
        {
            switch (msg.Severity)
            {
                case LogSeverity.Error:
                    {
                        logger.LogError(msg.ToString());
                        break;
                    }
                case LogSeverity.Warning:
                    {
                        logger.LogWarning(msg.ToString());
                        break;
                    }
                case LogSeverity.Info:
                    {
                        logger.LogInformation(msg.ToString());
                        break;
                    }
                case LogSeverity.Verbose:
                    {
                        logger.LogDebug(msg.ToString());
                        break;
                    }
                case LogSeverity.Critical:
                    {
                        logger.LogCritical(msg.ToString());
                        break;
                    }
                default:
                    {
                        logger.LogDebug(msg.ToString());
                        break;
                    }
            }
            return;
        }
    }
}
