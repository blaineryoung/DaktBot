using Daktbot.Common;
using Daktbot.Discord.Core;
using Daktbot.Storage;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Daktbot.Runner
{
    public static class ServiceBuilder
    {
        public static void BuildServices(IHostBuilder builder)
        { 
            builder.ConfigureServices((context, serviceCollection) => {
                serviceCollection.AddDiscordServices();
                serviceCollection.BindDiscordConfig(context.Configuration);
                serviceCollection.AddCommonServices();
                serviceCollection.Configure<JsonSerializerOptions>(options =>
                {
                    options.Converters.Add(new JsonStringEnumConverter());
                    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                }
                );
                serviceCollection.AddStorageServices(context.Configuration["CosmosDb:ConnectionString"]);
            });
            
        }
    }
}
