using Daktbot.Discord.Core.Client;
using Daktbot.Discord.Core.Commands;
using Daktbot.Discord.Core.Logic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core
{
    public static class ServiceBuilder
    {
        public static void AddDiscordServices(this IServiceCollection services)
        {
            services.AddSingleton<IDiscordBotClient, DiscordBotClient>();

            IEnumerable<Type> commands = BotUtilities.GetSubclasses(typeof(AbstractDiscordCommand));
            foreach (Type type in commands)
            {
                services.AddSingleton(type);
            }

            services.AddSingleton<IRaidPollCreator, RaidPollCreator>();
        }

        public static void BindDiscordConfig(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<DiscordOptions>(config.GetSection("Discord"));
        }
    }
}
