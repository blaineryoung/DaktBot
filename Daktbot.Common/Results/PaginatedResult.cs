using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Results
{
    public record PaginatedResult<TValue>
    {
        public PaginatedResult(IEnumerable<TValue> value, string? continuationToken)
        {
            Value = value;
            ContinuationToken = continuationToken;
        }

        public IEnumerable<TValue> Value { get; }
        public string? ContinuationToken { get; }
    }
}
