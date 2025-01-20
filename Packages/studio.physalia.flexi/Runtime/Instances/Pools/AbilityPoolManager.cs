using System.Collections.Generic;

namespace Physalia.Flexi
{
    internal class AbilityPoolManager
    {
        private readonly AbilitySystem abilitySystem;
        private readonly Dictionary<AbilityHandle, AbilityPool> pools;

        public AbilityPoolManager(AbilitySystem abilitySystem, int capacity = 0)
        {
            this.abilitySystem = abilitySystem;
            pools = new Dictionary<AbilityHandle, AbilityPool>(capacity);
        }

        public bool ContainsPool(AbilityHandle abilityHandle)
        {
            return pools.ContainsKey(abilityHandle);
        }

        public void CreatePool(AbilityHandle abilityHandle, int startSize)
        {
            if (abilityHandle == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Create pool failed! abilityData is null!");
                return;
            }

            if (startSize < 0)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Create pool failed! startSize: {startSize} < 0!");
                return;
            }

            if (pools.ContainsKey(abilityHandle))
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Create pool failed! key: {abilityHandle} already exists!");
                return;
            }

            var factory = new AbilityFactory(abilitySystem, abilityHandle);
            var pool = new AbilityPool(factory, startSize);
            pools.Add(abilityHandle, pool);
        }

        public void DestroyPool(AbilityHandle abilityHandle)
        {
            if (abilityHandle == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Destroy pool failed! abilityData is null!");
                return;
            }

            if (!pools.ContainsKey(abilityHandle))
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Destroy pool failed! key: {abilityHandle} does not exist!");
                return;
            }

            pools[abilityHandle].Clear();  // Note: Need to trigger Recover() for each Ability instance before releasing the reference.
            pools.Remove(abilityHandle);
        }

        public AbilityPool GetPool(AbilityHandle abilityHandle)
        {
            if (abilityHandle == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Get pool failed! abilityData is null!");
                return null;
            }

            bool hasPool = pools.TryGetValue(abilityHandle, out AbilityPool pool);
            if (!hasPool)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Get pool failed! key: {abilityHandle} does not exist!");
                return null;
            }

            return pool;
        }

        public Ability GetAbility(AbilityHandle abilityHandle)
        {
            if (abilityHandle == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Get ability failed! abilityData is null!");
                return null;
            }

            bool hasPool = pools.TryGetValue(abilityHandle, out AbilityPool pool);
            if (!hasPool)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Get ability failed! key: {abilityHandle} does not exist!");
                return null;
            }

            return pool.Get();
        }

        public bool ReleaseAbility(Ability ability)
        {
            if (ability == null)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Release ability failed! ability is null!");
                return false;
            }

            bool hasPool = pools.TryGetValue(ability.Handle, out AbilityPool pool);
            if (!hasPool)
            {
                Logger.Error($"[{nameof(AbilityPoolManager)}] Release ability failed! key: {ability.Handle} does not exist!");
                return false;
            }

            pool.Release(ability);
            return true;
        }
    }
}
