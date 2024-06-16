using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core.Client
{
    internal class DiscordClient : IDiscordClient
    {
        private readonly ILogger<DiscordClient> logger;
        private readonly DiscordOptions discordOptions;

        private DiscordSocketClient client;

        public DiscordClient(
            ILogger<DiscordClient> logger,
            IOptions<DiscordOptions> options)
        {
            this.logger = logger;
            this.discordOptions = options.Value;
        }

        public async Task Start()
        {
            client = new DiscordSocketClient();
            client.Log += Log;

            await client.LoginAsync(TokenType.Bot, discordOptions.GetToken());
            await client.StartAsync();
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
