namespace Physalia.Flexi
{
    internal abstract class ObjectInstanceFactory<T>
    {
        public abstract string Name { get; }

        public abstract T Create();
        public abstract void Reset(T instance);
    }
}
