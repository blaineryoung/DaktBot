using Microsoft.Azure.Cosmos;
using Daktbot.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Storage.Infrastructure
{
    public class CosmosDBHelper : ICosmosDBHelper
    {
        private readonly CosmosClient client;

        // First key: database name, second key: container name
        private Dictionary<string, Dictionary<string, Container>> _databaseCollection = new Dictionary<string, Dictionary<string, Container>>();

        private CosmosDBHelper(CosmosClient client, Dictionary<string, Dictionary<string, Container>> databaseCollection)
        {
            this.client = client;
            this._databaseCollection = databaseCollection;
        }

        internal static async Task<Result<ICosmosDBHelper, RequestError>> GetCosmosDBHelperInstance(string accountConnectionString) 
        {
            // Build the list of all storage services
            List<(string dbName, StorageContainerAttribute containerAttribute)> containers = new List<(string dbName, StorageContainerAttribute containerAttribute)>();

            Assembly assembly = Assembly.GetExecutingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                StorageContainerAttribute? attribute = type.GetCustomAttributes(typeof(StorageContainerAttribute), false).FirstOrDefault() as StorageContainerAttribute;
                if (attribute != null)
                {
                    containers.Add((attribute.DatabaseName, attribute));
                }
            }

            CosmosClient client = new(
                connectionString: accountConnectionString,
                clientOptions: new CosmosClientOptions
                    {
                        SerializerOptions = new CosmosSerializationOptions { 
                            PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase, 
                            IgnoreNullValues = true},
                    });
            Dictionary<string, Dictionary<string, Container>> databaseCollection = new Dictionary<string, Dictionary<string, Container>>();

            var groupedContainers = containers.GroupBy(x => x.dbName);
            foreach(var databaseSet in groupedContainers) 
            {
                try
                {
                    Database databaseToCreate = await client.CreateDatabaseIfNotExistsAsync(databaseSet.Key);
                    Dictionary<string, Container> databaseContainers = new Dictionary<string, Container>();

                    foreach (StorageContainerAttribute container in databaseSet.Select(y => y.containerAttribute))
                    {
                        Container c = await databaseToCreate.CreateContainerIfNotExistsAsync(new ContainerProperties(container.ContainerName, $"/{container.PartitionKey}"));
                        databaseContainers.Add(container.ContainerName, c);
                    }

                    databaseCollection.Add(databaseSet.Key, databaseContainers);
                }
                catch (Exception ex) 
                {
                    return new RequestError($"Could not create database {databaseSet.Key} reason: {ex.Message}", HttpStatusCode.InternalServerError);
                }
            }

            return new CosmosDBHelper(client, databaseCollection);
        }

        public Container GetContainer(string databaseName, string containerName)
        {
            return _databaseCollection[databaseName][containerName];
        }
    }
}
