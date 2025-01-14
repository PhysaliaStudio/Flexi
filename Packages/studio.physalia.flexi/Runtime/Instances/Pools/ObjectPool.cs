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
        private readonly List<T> allInstances;
        private readonly HashSet<T> usingInstances;
        private int rotationIndex;

        public int Size => size;
        public int UsingCount => usingInstances.Count;

        public ObjectPool(ObjectInstanceFactory<T> factory, int startSize, PoolExpandMethod expandMethod = PoolExpandMethod.OneAtATime)
        {
            this.factory = factory;
            this.expandMethod = expandMethod;

            size = startSize;
            allInstances = new List<T>(startSize);
            usingInstances = new HashSet<T>(startSize);
            for (int i = 0; i < startSize; i++)
            {
                CreateInstance();
            }
        }

        private void CreateInstance()
        {
            T instance = factory.Create();
            allInstances.Add(instance);
        }

        public T Get()
        {
            if (usingInstances.Count == size)
            {
                // Directly point to the future first new instance.
                rotationIndex = size;

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

            // Find an instance that is not being used.
            T instance = allInstances[rotationIndex];
            while (usingInstances.Contains(instance))
            {
                rotationIndex = (rotationIndex + 1) % size;
                instance = allInstances[rotationIndex];
            }

            usingInstances.Add(instance);
            rotationIndex = (rotationIndex + 1) % size;
            return instance;
        }

        public void Release(T instance)
        {
            if (instance == null)
            {
                Logger.Error($"[{nameof(ObjectPool<T>)}] Release null into the object pool. Pool Name: '{factory.Name}'");
                return;
            }

            bool success = usingInstances.Remove(instance);
            if (!success)
            {
                Logger.Error($"[{nameof(ObjectPool<T>)}] The released object is not belong to this pool. Pool Name: '{factory.Name}'");
                return;
            }

            factory.Reset(instance);
        }

        public void ReleaseAll()
        {
            foreach (T instance in usingInstances)
            {
                factory.Reset(instance);
            }

            usingInstances.Clear();
        }

        public void Clear()
        {
            ReleaseAll();
            allInstances.Clear();
            usingInstances.Clear();
            size = 0;
        }
    }
}
