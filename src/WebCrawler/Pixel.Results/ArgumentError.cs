namespace Pixel.Results
{
    public class ArgumentError<T> : Error<T>
    {
        public ArgumentError(T t) : this(t, string.Empty)
        {
        }

        public ArgumentError(T t, string msg) : base(t, msg)
        {

        }
    }
}