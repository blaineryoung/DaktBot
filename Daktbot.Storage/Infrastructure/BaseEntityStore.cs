using Daktbot.Common.Entities;
using Daktbot.Common.Results;
using Daktbot.Common.Storage;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Storage.Infrastructure
{
    internal abstract class BaseEntityStore<T> : IBaseEntityStore<T> where T : BaseEntity
    {
        protected Container Container { get; init; }

        protected ILogger Logger { get; init; }

        internal BaseEntityStore(ICosmosDBHelper helper, ILogger logger)
        {
            StorageContainerAttribute? attribute = this.GetType().GetCustomAttributes(typeof(StorageContainerAttribute), true).FirstOrDefault() as StorageContainerAttribute;
            if (attribute == null)
            {
                throw new InvalidOperationException("StorageContainerAttribute not defined for this class");
            }

            Container = helper.GetContainer(attribute.DatabaseName, attribute.ContainerName);
            Logger = logger;
        }

        public async Task<Result<T, RequestError>> Get(string id, string? partitionKey = null)
        {
            try
            {
                T response = await Container.ReadItemAsync<T>(
                        id: id,
                        partitionKey: new PartitionKey(string.IsNullOrWhiteSpace(partitionKey) ? id : partitionKey));

                if (response.IsDeleted)
                {
                    return new RequestError("Entity not found", HttpStatusCode.NotFound);
                }

                return response;
            } catch (CosmosException ex) 
            {
                return new RequestError(ex.Message, ex.StatusCode);
            }
        }

        public async Task<Result<PaginatedResult<T>, RequestError>> GetAll(string? partitionKey = null, string? continuationToken = null)
        {
            return await QueryWithContinuationToken("SELECT * FROM c where c.isDeleted = false", null, partitionKey, continuationToken);
        }

        protected async Task<Result<PaginatedResult<T>, RequestError>> QueryWithContinuationToken(
            string query,
            Dictionary<string, object>? parameters = null,
            string? partitionKey = null, 
            string? continuationToken = null)
        {
            try
            {
                List<T> results = new List<T>();
                string? responseToken = null;

                var parameterizedQuery = new QueryDefinition(query: query);

                if (parameters != null) 
                {
                    foreach (KeyValuePair<string, object> parameter in parameters) 
                    {
                        parameterizedQuery.WithParameter(parameter.Key, parameter.Value);
                    }
                }

                QueryRequestOptions queryOptions = new QueryRequestOptions();
                if (!string.IsNullOrEmpty(partitionKey))
                {
                    queryOptions.PartitionKey = new PartitionKey(partitionKey);
                }

                using (FeedIterator<T> resultSetIterator = Container.GetItemQueryIterator<T>(
                    queryDefinition: parameterizedQuery,
                    requestOptions: queryOptions,
                    continuationToken: continuationToken))
                {
                    // Execute query and get 1 item in the results. Then, get a continuation token to resume later
                    if (resultSetIterator.HasMoreResults)
                    {
                        FeedResponse<T> response = await resultSetIterator.ReadNextAsync();


                        results.AddRange(response);

                        // Get continuation token once we've gotten > 0 results. 
                        if (resultSetIterator.HasMoreResults)
                        {
                            responseToken = response.ContinuationToken;
                        }
                    }
                }

                return new PaginatedResult<T>(results, responseToken);
            } catch (CosmosException ex) 
            {
                return new RequestError(ex.Message, ex.StatusCode);
            }
        }

        public async Task<Result<T, RequestError>> Upsert(T entity, string? partitionKey = null)
        {
            try
            {
                T updatedEntity = SetCommonFields(entity);
                PartitionKey key = new PartitionKey(string.IsNullOrWhiteSpace(partitionKey) ? entity.Id : partitionKey);

                ItemRequestOptions? requestOptions = null;
                if (!string.IsNullOrWhiteSpace(entity.Etag)) 
                {
                    requestOptions = new ItemRequestOptions()
                    {
                        IfMatchEtag = entity.Etag,
                    };
                }

                T upsertedItem = await Container.UpsertItemAsync<T>(
                    item: updatedEntity,
                    partitionKey: key,
                    requestOptions: requestOptions);
                return upsertedItem;
            }
            catch (CosmosException ex) 
            {
                return new RequestError(ex.Message, ex.StatusCode);
            }
        }

        public virtual async Task<Result<HttpStatusCode, RequestError>> Delete(string entityId, string? partitionKey = null)
        {
            Result<T, RequestError> fetchResult = await this.Get(entityId, partitionKey);
            T? entity = null;

            bool succeeded = fetchResult.Match<bool>(
                m => { entity = m; return true; },
                e => false );
            if (!succeeded || entity == null) 
            {
                return fetchResult.CastError<HttpStatusCode>();
            }

            if (entity.IsDeleted == true)
            {
                return HttpStatusCode.NoContent;
            }

            Result<T, RequestError> upsertResult = await this.Upsert(entity with { IsDeleted = true }, partitionKey);
            return upsertResult.IsError ? upsertResult.CastError<HttpStatusCode>() : HttpStatusCode.NoContent;
        }

        private T SetCommonFields(T inputObject)
        {
            DateTime now = DateTime.UtcNow;

            return inputObject with
            {
                CreationDate = inputObject.CreationDate == DateTime.MinValue ? now : inputObject.CreationDate,
                UpdatedDate = now,
                RetentionCleanupDate = now.AddYears(7)
            };
        }
    }
}
