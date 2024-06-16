using Daktbot.Discord.Core.Client;

namespace Daktbot.Runner
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IDiscordClient discordClient;

        public Worker(
            ILogger<Worker> logger,
            IDiscordClient discordClient)
        {
            _logger = logger;
            this.discordClient = discordClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Connecting to discord");

            await discordClient.Start();

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}