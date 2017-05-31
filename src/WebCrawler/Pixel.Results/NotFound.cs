namespace Pixel.Results
{
    public class NotFound<T> : Error<T>
    {
        public NotFound(T t) : this(t, string.Empty)
        {
        }

        public NotFound(T t, string msg) : base(t, msg)
        {
        }
    }
}