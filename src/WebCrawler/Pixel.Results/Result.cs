namespace Pixel.Results
{
    
    public class Result<T> : Result, IResult<T>
    {
        public new T Value => (T)base.Value;
        protected Result()
        {
        }

        protected Result(T t) : base(t, string.Empty)
        {
        }

        protected Result(T t, string msg) : base(t, msg)
        {
        }
    }

    public class Result : IResult
    {
        public string Message { get; }
        public object Value { get; }

        protected Result()
        {
        }

        protected Result(object value, string message)
        {
            this.Value = value;
            this.Message = message;
        }

        protected Result(string message)
        {
            this.Message = message;
        }
    }
}