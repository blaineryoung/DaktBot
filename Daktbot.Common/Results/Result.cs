using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daktbot.Common.Results
{
    public readonly struct Result<TValue, TError> where TError : RequestError
    {
        private readonly TValue? _value;
        private readonly TError? _error;

        public bool IsError { get; init; }

        private Result(TValue value)
        {
            IsError = false;
            _value = value;
            _error = default;
        }

        private Result(TError error)
        {
            IsError = true;
            _error = error;
            _value = default;
        }

        public static implicit operator Result<TValue, TError>(TValue value) => new(value);
        public static implicit operator Result<TValue, TError>(TError error) => new(error);

        public TResult Match<TResult>(
            Func<TValue, TResult> success,
            Func<TError, TResult> failure) => !IsError ? success(_value!) : failure(_error!);

        public Result<T, RequestError> CastError<T>()
        {
            return _error ?? new RequestError("Attempted to cast a non error", System.Net.HttpStatusCode.BadRequest);
        }
    }
}
