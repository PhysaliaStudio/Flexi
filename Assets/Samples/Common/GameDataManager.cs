using System;
using System.Collections;
using System.Collections.Generic;

namespace Physalia.Flexi.Samples
{
    public interface IHasGameId
    {
        int Id { get; }
    }

    public class GameDataManager
    {
        private readonly AssetManager assetManager;
        private readonly Dictionary<Type, IDictionary> dataTables = new();

        public GameDataManager(AssetManager assetManager)
        {
            this.assetManager = assetManager;
        }

        public void LoadAllData<T>(string path) where T : UnityEngine.Object, IHasGameId
        {
            var table = GetOrCreateTable<T>();
            T[] assets = assetManager.LoadAll<T>(path);
            for (var i = 0; i < assets.Length; i++)
            {
                T data = assets[i];
                table.Add(data.Id, data);
            }
        }

        public T GetData<T>(int id)
        {
            IReadOnlyDictionary<int, T> table = GetTable<T>();
            if (table.ContainsKey(id))
            {
                return table[id];
            }
            else
            {
                return default;
            }
        }

        private Dictionary<int, T> GetOrCreateTable<T>()
        {
            if (dataTables.TryGetValue(typeof(T), out IDictionary genericTable))
            {
                return genericTable as Dictionary<int, T>;
            }
            else
            {
                var table = new Dictionary<int, T>();
                dataTables.Add(typeof(T), table);
                return table;
            }
        }

        public IReadOnlyDictionary<int, T> GetTable<T>()
        {
            var type = typeof(T);
            if (!dataTables.ContainsKey(type))
            {
                throw new Exception($"[{nameof(GameDataManager)}] Cannot find the table of type: {type.Name}");
            }

            return dataTables[type] as Dictionary<int, T>;
        }
    }
}
