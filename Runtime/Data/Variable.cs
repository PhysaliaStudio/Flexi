using System;

namespace Physalia.AbilitySystem
{
    public abstract class Variable
    {
        public abstract Type ValueType { get; }

        public object Value
        {
            get
            {
                return GetValueBoxed();
            }
            set
            {
                SetValueBoxed(value);
            }
        }

        protected abstract object GetValueBoxed();
        protected abstract void SetValueBoxed(object value);
    }

    public sealed class Variable<T> : Variable
    {
        private T value;

        public override Type ValueType => typeof(T);

        public new T Value
        {
            get
            {
                return GetValue();
            }
            set
            {
                SetValue(value);
            }
        }

        public Variable()
        {
            value = default;
        }

        public Variable(T value)
        {
            this.value = value;
        }

        protected override object GetValueBoxed()
        {
            return value;
        }

        protected override void SetValueBoxed(object value)
        {
            this.value = (T)value;
        }

        private T GetValue()
        {
            return value;
        }

        private void SetValue(T value)
        {
            this.value = value;
        }
    }
}
