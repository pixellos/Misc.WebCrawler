namespace Pixel.Results
{
    public class Succeeded<T> : Result<T>, ISucceeded<T>
    {
        public static implicit operator T(Succeeded<T> succeeded)
        {
            return succeeded.Value;
        }

        public Succeeded(T t) : base(t, string.Empty)
        {
        }

        public Succeeded(T t, string msg) : base(t, msg)
        {
        }
    }

    public class Succeeded : Result, ISucceeded
    {
        public Succeeded()
        {
        }

        public Succeeded(object value) : base(value, string.Empty)
        {
        }

        public Succeeded(object value, string msg) : base(value, msg)
        {
        }
    }
}