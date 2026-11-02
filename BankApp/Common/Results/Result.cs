namespace BankApp.Common.Results;

internal class Result<TValue, TError> where TError : struct, Enum // struct contraint makes enum nullable.
{
    public bool IsSuccess { get; }
    public TValue? Value { get; }
    public TError? Error { get; }

    private Result(bool isSuccess, TValue? value, TError? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<TValue, TError> Success(TValue value)
    {
        return new Result<TValue, TError>
        (
            true,
            value,
            default
        );
    }

    public static Result<TValue, TError> Failure(TError error)
    {
        return new Result<TValue, TError>
        (
            false,
            default,
            error
        );
    }
}