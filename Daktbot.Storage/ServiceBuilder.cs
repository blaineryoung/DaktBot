using Daktbot.Common.Results;
using Microsoft.Extensions.DependencyInjection;
using Daktbot.Storage.Infrastructure;
using Daktbot.Common.Stores;
using Daktbot.Storage.EntityStores;

namespace Daktbot.Storage
{
    public static class ServiceBuilder
    {
        public static void AddStorageServices(this IServiceCollection services, string cosmosDbEndpoint)
        {
            Result<ICosmosDBHelper, RequestError> initializeResult = CosmosDBHelper.GetCosmosDBHelperInstance(cosmosDbEndpoint).Result;

            initializeResult.Match(
                m => services.AddSingleton<ICosmosDBHelper>(m),
                failed => throw new Exception(failed.ErrorMessage));

            services.AddSingleton<IPersonStore, PersonStore>();
        }
    }
}