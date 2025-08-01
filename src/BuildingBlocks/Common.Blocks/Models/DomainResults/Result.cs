﻿namespace Common.Blocks.Models.DomainResults
{
    public class Result
    {
        public bool IsSuccess { get; }

        public bool IsFailure => !IsSuccess;

        public Error Error { get; }

        protected Result(bool isSuccess, Error error)
        {
            if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
            {
                throw new ArgumentException("Invalid error", nameof(error));
            }

            IsSuccess = isSuccess;
            Error = error;
        }

        public static Result Success() => new(true, Error.None);

        public static Result Failure(Error error) => new(false, error);

        public static Result<T> Success<T>(T value) => new(value, true, Error.None);

        public static Result<T> Failure<T>(Error error) => new(default, false, error);

        public static Result<T> Create<T>(T? value) =>
            value is not null ? Success(value) : Failure<T>(Error.NullValue);

        //public T Match<T>(Func<T> onSuccess, Func<Error, T> onFailure)
        //{
        //    return this.IsSuccess ? onSuccess() : onFailure(this.Error);
        //}

        public T Match<T>(Func<T> onSuccess, Func<Error, T> onFailure)
        {
            return this.IsSuccess ? onSuccess() : onFailure(this.Error);
        }
    }
}
