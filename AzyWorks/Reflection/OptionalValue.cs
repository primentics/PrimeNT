namespace AzyWorks.Reflection
{
    public class OptionalValue<TValue>
    {
        private TValue _value;
        private bool _isNull;

        public bool HasValue { get => !_isNull; }

        public TValue Value
        {
            get
            {
                if (_isNull)
                    return default;

                return _value;
            }
        }

        public void SetValue(TValue value)
        {
            _value = value;
            _isNull = false;
        }

        internal OptionalValue(TValue value) { _value = value; }
        internal OptionalValue() { _isNull = true; }

        public static OptionalValue<TValue> FromValue(TValue value)
            => new OptionalValue<TValue>(value);
        public static OptionalValue<TValue> FromNull()
            => new OptionalValue<TValue>();
    }
}