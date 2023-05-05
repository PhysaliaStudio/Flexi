using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Physalia.Flexi.Samples.CardGame
{
    public class GameObjectPool<T> where T : Component
    {
        private readonly T original;
        private readonly ObjectPool<T> pool;
        private readonly Transform root;

        private readonly HashSet<T> activeElements = new();

        public GameObjectPool(T original, Transform poolParent, string poolName, int defaultCount, int maxCount)
        {
            this.original = original;
            if (string.IsNullOrEmpty(poolName))
            {
                poolName = $"Pool [{typeof(T).Name}]";
            }

            if (poolParent != null)
            {
                if (poolParent is RectTransform)
                {
                    root = new GameObject(poolName, typeof(RectTransform)).transform;
                }
                else
                {
                    root = new GameObject(poolName).transform;
                }

                root.SetParent(poolParent, false);
            }
            else
            {
                root = new GameObject(poolName).transform;
            }

            pool = new ObjectPool<T>(OnCreatePoolObject, OnGetPoolObject, OnReleasePoolObject, OnDestroyPoolObject, true, defaultCount, maxCount);
        }

        private T OnCreatePoolObject()
        {
            T instance = Object.Instantiate(original, root);
            return instance;
        }

        private void OnGetPoolObject(T element)
        {
            element.gameObject.SetActive(true);
        }

        private void OnReleasePoolObject(T element)
        {
            element.transform.SetParent(root);
            element.gameObject.SetActive(false);
        }

        private void OnDestroyPoolObject(T element)
        {
            Object.Destroy(element.gameObject);
        }

        public T Get()
        {
            T element = pool.Get();
            activeElements.Add(element);
            return element;
        }

        public void Release(T element)
        {
            pool.Release(element);
            activeElements.Remove(element);
        }

        public void ReleaseAll()
        {
            foreach (T element in activeElements)
            {
                pool.Release(element);
            }
            activeElements.Clear();
        }
    }
}
