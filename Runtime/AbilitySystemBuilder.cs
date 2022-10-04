using UnityEngine;

namespace Physalia.AbilitySystem.Tests
{
    public class AbilitySystemBuilder
    {
        private StatDefinitionListAsset asset;

        public AbilitySystem Build()
        {
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
                Debug.LogWarning($"[{nameof(AbilitySystemBuilder)}] The stat definition asset is null. Internally created an empty one.");
            }

            return new AbilitySystem(asset);
        }

        public void SetStatDefinitions(StatDefinitionListAsset asset)
        {
            this.asset = asset;
        }
    }
}
