namespace PetDoctor.API.Application;

public class CommandResult
{
    public static CommandResult<T, T1> Success<T, T1>(T result) where T1 : class => CommandResult<T, T1>.Success(result);
    public static CommandResult<T, T1> Failed<T, T1>(T1 error) where T1 : class => CommandResult<T, T1>.Failed(error);
}

public class CommandResult<TReturn, TError> : CommandResult where TError : class
{
    public bool Succeeded => Error == null;
    public TError? Error { get; }
    public TReturn? Payload { get; }

    public CommandResult(TError? error, TReturn? payload)
    {
        Error = error;
        Payload = payload;
    }

    public static CommandResult<TReturn, TError> Success(TReturn payload) => new(null, payload);
    public static CommandResult<TReturn, TError> Failed(TError error) => new(error, default);
}