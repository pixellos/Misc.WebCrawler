namespace Pixel.Results
{
    public class Error<T> : Result<T>, IError<T>, IError
    {
        public new T Value => (T)base.Value;

        public Error(T t) : this(t, string.Empty)
        {
        }

        public Error(T t, string msg) : base(t, msg)
        {
        }
    }

    public class Error : Result, IError
    {
        public Error()
        {
        }
        
        public Error(object value) : this(value, string.Empty)
        {
        }

        public Error(object value, string msg) : base(value, msg)
        {
        }
    }
}