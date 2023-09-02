using System.Collections.Generic;

namespace Physalia.Flexi
{
    public enum PoolExpandMethod
    {
        OneAtATime, Double
    }

    internal class ObjectPool<T> where T : class
    {
        private readonly ObjectInstanceFactory<T> factory;
        private readonly PoolExpandMethod expandMethod;

        private int size;
        private readonly List<T> remainedInstances = new();
        private readonly List<T> usingInstances = new();

        public int Size => size;
        public int UsingCount => usingInstances.Count;
        public IReadOnlyList<T> RemainedInstances => remainedInstances;

        public ObjectPool(ObjectInstanceFactory<T> factory, int startSize, PoolExpandMethod expandMethod = PoolExpandMethod.OneAtATime)
        {
            this.factory = factory;
            this.expandMethod = expandMethod;

            size = startSize;
            for (int i = 0; i < startSize; i++)
            {
                CreateInstance();
            }
        }

        private void CreateInstance()
        {
            T instance = factory.Create();
            remainedInstances.Add(instance);
        }

        public T Get()
        {
            if (remainedInstances.Count == 0)
            {
                // Create new object by the expand method
                if (expandMethod == PoolExpandMethod.OneAtATime)
                {
                    size++;
                    CreateInstance();
                }
                else if (expandMethod == PoolExpandMethod.Double)
                {
                    int expandAmount = size;
                    size += expandAmount;
                    for (int i = 0; i < expandAmount; i++)
                    {
                        CreateInstance();
                    }
                }
            }

            T instance = remainedInstances[0];
            remainedInstances.RemoveAt(0);
            usingInstances.Add(instance);
            return instance;
        }

        public void Release(T instance)
        {
            if (instance == null)
            {
                Logger.Error($"[{nameof(ObjectPool<T>)}] Release null into the object pool. Pool Name: '{factory.Name}'");
                return;
            }

            if (!usingInstances.Contains(instance))
            {
                Logger.Error($"[{nameof(ObjectPool<T>)}] The released object is not belong to this pool. Pool Name: '{factory.Name}'");
                return;
            }

            factory.Reset(instance);
            remainedInstances.Add(instance);
            usingInstances.Remove(instance);
        }

        public void ReleaseAll()
        {
            while (usingInstances.Count > 0)
            {
                T instance = usingInstances[0];
                Release(instance);
            }
        }

        public void Clear()
        {
            ReleaseAll();
            remainedInstances.Clear();
            usingInstances.Clear();
            size = 0;
        }
    }
}
