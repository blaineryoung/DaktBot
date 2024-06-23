using Daktbot.Common.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Utilities
{
    public static class LogUtilities
    {
        public static bool LogRequestError(this ILogger logger, RequestError error, string message, params object?[] args)
        {
            string updatedMessage = $"{message} - error was {{requestError}}";
            logger.LogError(updatedMessage, args.Append(error.ErrorMessage).ToArray());

            // For convenient use in request.match
            return false;
        }
    }
}
