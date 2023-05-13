using System.Collections.Generic;

namespace Physalia.Flexi
{
    internal class AbilityPoolManager
    {
        private readonly AbilitySystem abilitySystem;
        private readonly Dictionary<AbilityData, AbilityPool> pools;

        internal AbilityPoolManager(AbilitySystem abilitySystem, int capacity = 0)
        {
            this.abilitySystem = abilitySystem;
            pools = new Dictionary<AbilityData, AbilityPool>(capacity);
        }

        internal bool ContainsPool(AbilityData abilityData)
        {
            return pools.ContainsKey(abilityData);
        }

        internal void CreatePool(AbilityData abilityData, int startSize)
        {
            if (abilityData == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Create pool failed! abilityData is null!");
                return;
            }

            if (startSize < 0)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Create pool failed! startSize: {startSize} < 0!");
                return;
            }

            if (pools.ContainsKey(abilityData))
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Create pool failed! key: 'AbilityData({abilityData.name})' already exists!");
                return;
            }

            var factory = new AbilityFactory(abilitySystem, abilityData);
            var pool = new AbilityPool(factory, startSize);
            pools.Add(abilityData, pool);
        }

        internal void DestroyPool(AbilityData abilityData)
        {
            if (abilityData == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Destroy pool failed! abilityData is null!");
                return;
            }

            if (!pools.ContainsKey(abilityData))
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Destroy pool failed! key: 'AbilityData({abilityData.name})' does not exist!");
                return;
            }

            pools[abilityData].Clear();  // Note: Need to trigger Recover() for each Ability instance before releasing the reference.
            pools.Remove(abilityData);
        }

        internal AbilityPool GetPool(AbilityData abilityData)
        {
            if (abilityData == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Get pool failed! abilityData is null!");
                return null;
            }

            if (!pools.ContainsKey(abilityData))
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Get pool failed! key: 'AbilityData({abilityData.name})' does not exist!");
                return null;
            }

            return pools[abilityData];
        }

        internal Ability GetAbility(AbilityData abilityData)
        {
            if (abilityData == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Get ability failed! abilityData is null!");
                return null;
            }

            if (!pools.ContainsKey(abilityData))
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Get ability failed! key: 'AbilityData({abilityData.name})' does not exist!");
                return null;
            }

            AbilityPool pool = pools[abilityData];
            return pool.Get();
        }

        internal void ReleaseAbility(Ability ability)
        {
            if (ability == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Release ability failed! ability is null!");
                return;
            }

            if (!pools.ContainsKey(ability.Data))
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Release ability failed! key: 'AbilityData({ability.Data.name})' does not exist!");
                return;
            }

            AbilityPool pool = pools[ability.Data];
            pool.Release(ability);
        }
    }
}
