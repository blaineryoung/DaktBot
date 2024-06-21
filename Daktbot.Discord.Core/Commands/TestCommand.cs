using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core.Commands
{
    internal class TestCommand : AbstractDiscordCommand
    {
        internal override ILogger Logger { get; }

        internal override string Name => "test-command";

        internal override string Description => "Quick test to ensure the bot is working";

        public TestCommand(
            ILogger<TestCommand> logger)
        {
            Logger = logger;
        }

        internal override async Task HandleCommand(SocketSlashCommand command)
        {
            await command.RespondAsync($"Test successful!  You executed {command.Data.Name}");
        }
    }
}
