using Microsoft.Extensions.Configuration;

namespace Daktbot.Discord.Core
{
    public class DiscordOptions : IDiscordOptions
    {
        public bool DevEnvironment { get; set; }
        public string Token { get; set; }

        public string DevToken { get; set; }

        public string GetToken() => DevEnvironment ? DevToken : Token;
    }
}