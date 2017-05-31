namespace Pixel.Results
{
    public interface IError<T> : IResult<T>, IError
    {
    }

    public interface IError : IResult
    {
        string Message { get; }
    }
}