using System.Text.Json.Serialization;

namespace WhoHolds.Core.Common.Models;

public readonly struct Result
{
    public Result(bool succeeded, params string[] errors)
    {
        if (succeeded && errors.Length > 0)
            throw new ArgumentException($"Succeeded {nameof(Result)} cannot contain errors.");

        Succeeded = succeeded;
        Errors = errors;
    }

    public bool Succeeded { get; }
    public string[] Errors { get; }

    public static Result Success() => new(true);

    public static Result<T> Success<T>(T value) => new(true, value);

    public static ResultFailure Failure(params string[] errors) => new(errors);

    public static implicit operator Result(ResultFailure failure) => new(false, failure.Errors);
}

public readonly struct Result<T>
{
    public Result(bool succeeded, T value, params string[] errors)
    {
        if (succeeded && errors.Length > 0)
            throw new ArgumentException($"Succeeded {nameof(Result)} cannot contain errors.");

        Succeeded = succeeded;
        Value = value;
        Errors = errors;
    }

    public bool Succeeded { get; }
    public T Value
    {
        get
        {
            if (!Succeeded)
            {
                throw new InvalidOperationException("Failed result does not have value.");
            }

            return field;
        }
    }
    public string[] Errors { get; }

    public static implicit operator Result<T>(ResultFailure failure)
    {
        return new Result<T>(false, default!, failure.Errors);
    }
}

public readonly struct ResultFailure(params string[] errors)
{
    public string[] Errors { get; } = errors;
}
