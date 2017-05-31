namespace Pixel.Results
{
    public class Exist<T> : Error<T>
    {
        public Exist(T t) : this(t, string.Empty)
        {
        }

        public Exist(T t, string msg) : base(t, msg)
        {

        }
    }
}