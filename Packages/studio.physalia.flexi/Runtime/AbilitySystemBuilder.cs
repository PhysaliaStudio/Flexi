using UnityEngine;

namespace Physalia.Flexi
{
    public class AbilitySystemBuilder
    {
        private StatDefinitionListAsset asset;
        private IModifierAlgorithm modifierAlgorithm;
        private AbilityFlowRunner runner;

        public AbilitySystem Build()
        {
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<StatDefinitionListAsset>();
                Logger.Warn($"[{nameof(AbilitySystemBuilder)}] The stat definition asset is null. Internally created an empty one.");
            }

            if (modifierAlgorithm == null)
            {
                modifierAlgorithm = new DefaultModifierAlgorithm();
            }

            if (runner == null)
            {
                runner = new LifoQueueRunner();
            }

            Logger.Info($"[{nameof(AbilitySystemBuilder)}] Runner Type: {runner.GetType().Name}");

            return new AbilitySystem(asset, modifierAlgorithm, runner);
        }

        public void SetStatDefinitions(StatDefinitionListAsset asset)
        {
            this.asset = asset;
        }

        public void SetModifierAlgorithm(IModifierAlgorithm modifierAlgorithm)
        {
            this.modifierAlgorithm = modifierAlgorithm;
        }

        public void SetRunner(AbilityFlowRunner runner)
        {
            this.runner = runner;
        }
    }
}
