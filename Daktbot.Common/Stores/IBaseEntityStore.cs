using Daktbot.Common.Entities;
using Daktbot.Common.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Storage
{
    public interface IBaseEntityStore<T> where T : BaseEntity
    {
        Task<Result<T, RequestError>> Get(string id, string? partitionKey = null);

        Task<Result<T, RequestError>> Upsert(T entity, string? partitionKey = null);

        Task<Result<PaginatedResult<T>, RequestError>> GetAll(string? partitionKey = null, string? continuationToken = null);

        Task<Result<HttpStatusCode, RequestError>> Delete(string entityId, string? partitionKey = null);
    }
}
