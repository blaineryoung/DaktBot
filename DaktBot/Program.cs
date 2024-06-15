using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace Daktbot.Runner
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            IHostBuilder host = Host.CreateDefaultBuilder(args)
            .ConfigureServices(services =>
            {
                services.AddHostedService<Worker>();

            });

            IConfigurationRoot? config = null;
            host.ConfigureAppConfiguration((context, c) =>
            {
                config = c.Build();
                c.AddAzureKeyVault(
                    new Uri($"https://{config["KeyVaultName"]}.vault.azure.net/"),
                    new DefaultAzureCredential());
                config = c.Build();
            });

            host.ConfigureLogging((context, c) =>
            {
                c.AddApplicationInsights(config["AppInsights:LoggingKey"]);
            });

            host.Build().Run();
        }
    }
}