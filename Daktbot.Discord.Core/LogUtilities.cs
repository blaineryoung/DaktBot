using Daktbot.Common.Results;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Discord.Core
{
    public static class LogUtilities
    {
        public static void LogRequestError(this ILogger logger, RequestError error, string message, params object?[] args)
        {
            string updatedMessage = $"{message} - error was {{requestError}}";
            logger.LogError(updatedMessage, args.Append(error.ErrorMessage).ToArray());
        }
    }
}
