using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core.Client
{
    public interface IDiscordBotClient
    {
        DiscordSocketClient Client { get; }

        Task RegisterCommands();
        Task Start();
    }
}
