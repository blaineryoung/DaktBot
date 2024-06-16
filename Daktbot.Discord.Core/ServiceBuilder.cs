using Daktbot.Discord.Core.Client;
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
            services.AddSingleton<IDiscordClient, DiscordClient>();
        }

        public static void BindDiscordConfig(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<DiscordOptions>(config.GetSection("Discord"));
        }
    }
}
