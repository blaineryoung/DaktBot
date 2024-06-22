using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Results
{
    public record RequestError
    {
        public string ErrorMessage { get; init; }

        public HttpStatusCode StatusCode { get; init; }

        public Exception? Exception { get; init; } = null;

        public RequestError(string message, HttpStatusCode code)
        {
            this.ErrorMessage = message;
            this.StatusCode = code;
        }

        public RequestError(Exception exception) 
        {
            this.ErrorMessage = exception.Message;
            this.StatusCode = HttpStatusCode.InternalServerError;
            this.Exception = exception;
        }
    }
}
