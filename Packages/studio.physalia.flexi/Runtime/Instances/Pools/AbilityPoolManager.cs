using System.Collections.Generic;

namespace Physalia.Flexi
{
    internal class AbilityPoolManager
    {
        private readonly AbilitySystem abilitySystem;
        private readonly Dictionary<AbilityDataSource, AbilityPool> pools;

        internal AbilityPoolManager(AbilitySystem abilitySystem, int capacity = 0)
        {
            this.abilitySystem = abilitySystem;
            pools = new Dictionary<AbilityDataSource, AbilityPool>(capacity);
        }

        internal bool ContainsPool(AbilityDataSource abilityDataSource)
        {
            return pools.ContainsKey(abilityDataSource);
        }

        internal void CreatePool(AbilityDataSource abilityDataSource, int startSize)
        {
            if (abilityDataSource == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Create pool failed! abilityData is null!");
                return;
            }

            if (startSize < 0)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Create pool failed! startSize: {startSize} < 0!");
                return;
            }

            if (pools.ContainsKey(abilityDataSource))
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Create pool failed! key: {abilityDataSource} already exists!");
                return;
            }

            var factory = new AbilityFactory(abilitySystem, abilityDataSource);
            var pool = new AbilityPool(factory, startSize);
            pools.Add(abilityDataSource, pool);
        }

        internal void DestroyPool(AbilityDataSource abilityDataSource)
        {
            if (abilityDataSource == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Destroy pool failed! abilityData is null!");
                return;
            }

            if (!pools.ContainsKey(abilityDataSource))
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Destroy pool failed! key: {abilityDataSource} does not exist!");
                return;
            }

            pools[abilityDataSource].Clear();  // Note: Need to trigger Recover() for each Ability instance before releasing the reference.
            pools.Remove(abilityDataSource);
        }

        internal AbilityPool GetPool(AbilityDataSource abilityDataSource)
        {
            if (abilityDataSource == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Get pool failed! abilityData is null!");
                return null;
            }

            if (!pools.ContainsKey(abilityDataSource))
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Get pool failed! key: {abilityDataSource} does not exist!");
                return null;
            }

            return pools[abilityDataSource];
        }

        internal Ability GetAbility(AbilityDataSource abilityDataSource)
        {
            if (abilityDataSource == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Get ability failed! abilityData is null!");
                return null;
            }

            if (!pools.ContainsKey(abilityDataSource))
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Get ability failed! key: {abilityDataSource} does not exist!");
                return null;
            }

            AbilityPool pool = pools[abilityDataSource];
            return pool.Get();
        }

        internal void ReleaseAbility(Ability ability)
        {
            if (ability == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Release ability failed! ability is null!");
                return;
            }

            if (!pools.ContainsKey(ability.DataSource))
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Release ability failed! key: {ability.DataSource} does not exist!");
                return;
            }

            AbilityPool pool = pools[ability.DataSource];
            pool.Release(ability);
        }
    }
}
