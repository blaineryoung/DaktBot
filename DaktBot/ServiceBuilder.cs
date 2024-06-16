using Daktbot.Discord.Core;

namespace Daktbot.Runner
{
    public static class ServiceBuilder
    {
        public static void BuildServices(IHostBuilder builder)
        {
            builder.ConfigureServices((context, serviceCollection) => {
                serviceCollection.AddDiscordServices();
                serviceCollection.BindDiscordConfig(context.Configuration);
            });
        }
    }
}
